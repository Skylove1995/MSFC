using System.Diagnostics;
using System.Text;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using MSFC.Models;
using Color = System.Drawing.Color;

namespace MSFC
{
    /// <summary>
    /// Sequential scan queue: scanner → queue → send ONE PID to SFC → wait for the SFC result → next PID.
    /// Block / EBR / NG / timeout pause the queue behind a popup until the operator confirms.
    /// Scans keep being queued while the popup is open.
    /// </summary>
    public partial class Form1
    {
        private readonly record struct ScanItem(string Pid, int Generation);

        private readonly Channel<ScanItem> _scanQueue =
            Channel.CreateUnbounded<ScanItem>(new UnboundedChannelOptions { SingleReader = true });
        private readonly HashSet<string> _queuedPids = new(StringComparer.OrdinalIgnoreCase);
        private readonly SemaphoreSlim _sfcGate = new(1, 1); // one PID in SFC or one print at a time
        private int _queueGeneration;                         // bumped by "Clear" to drop stale items
        private volatile bool _alertOpen;
        private Task? _scanWorkerTask;

        private const int DefaultSfcTimeoutMs = 3000;
        private const int SfcPollMs = 50;
        private const int ResultStableMs = 100;   // SFC may briefly show the new PID with the previous result
        private const int SamePidSettleMs = 600;  // re-scan of the PID already on screen: ignore the stale result

        private void StartScanWorker() => _scanWorkerTask ??= Task.Run(ScanWorkerAsync);

        private void ProcessScannedData(string rawInput)
        {
            string pid = Normalize(rawInput);
            if (string.IsNullOrWhiteSpace(pid)) return;

            if (pid.Equals("CLEAR", StringComparison.OrdinalIgnoreCase))
            {
                ClearScanQueue();
                BeginInvoke(new Action(resetUI));
                return;
            }

            EnqueueScan(pid);
        }

        private void EnqueueScan(string pid)
        {
            if (IsCounted(pid))
            {
                SetQueueStatus($"⏭ {pid} ya fue contado en este lote", Color.DarkOrange);
                return;
            }

            int pending;
            lock (_queuedPids)
            {
                if (!_queuedPids.Add(pid))
                {
                    SetQueueStatus($"⏭ {pid} ya está en cola", Color.DarkOrange);
                    return;
                }
                pending = _queuedPids.Count;
            }

            _scanQueue.Writer.TryWrite(new ScanItem(pid, Volatile.Read(ref _queueGeneration)));
        }

        private void ClearScanQueue()
        {
            Interlocked.Increment(ref _queueGeneration);
            lock (_queuedPids) _queuedPids.Clear();
        }

        private int QueuedCount()
        {
            lock (_queuedPids) return _queuedPids.Count;
        }

        private bool IsCounted(string pid)
        {
            lock (_pendingLock) return _pendingPids.Contains(pid);
        }

        private async Task ScanWorkerAsync()
        {
            await foreach (var item in _scanQueue.Reader.ReadAllAsync().ConfigureAwait(false))
            {
                await _sfcGate.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (item.Generation != Volatile.Read(ref _queueGeneration)) continue;

                    // Removed before processing so the operator can re-scan (retry) this PID while a popup is open
                    lock (_queuedPids) _queuedPids.Remove(item.Pid);
                    await ProcessQueuedPidAsync(item.Pid).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ScanWorker Error] {item.Pid}: {ex}");
                    await ShowScanAlertAsync("ERROR INTERNO / INTERNAL ERROR", item.Pid, ex.Message).ConfigureAwait(false);
                }
                finally
                {
                    _sfcGate.Release();
                }
            }
        }

        private async Task ProcessQueuedPidAsync(string pid)
        {
            if (IsCounted(pid)) return; // same PID scanned twice in a row; first one already OK

            var precheck = await PrecheckAsync(pid).ConfigureAwait(false);
            if (precheck != null)
            {
                await ShowScanAlertAsync(precheck.Value.Title, pid, precheck.Value.Detail).ConfigureAwait(false);
                return;
            }

            if (_portSFC == null || !_portSFC.IsOpen)
            {
                await ShowScanAlertAsync("PUERTO SFC CERRADO / SFC PORT CLOSED", pid,
                    $"No se pudo enviar el PID: el puerto '{_portSFC?.PortName}' no está abierto.\r\n\r\n" +
                    "Could not send the PID: the SFC COM port is not open.").ConfigureAwait(false);
                return;
            }

            if (!_isWatching)
            {
                // Not linked with the SFC screen (setting not confirmed) → send only, like before
                _portSFC.WriteLine(pid);
                return;
            }

            var before = await ReadSfcHeadAsync().ConfigureAwait(false);
            _portSFC.WriteLine(pid);

            int timeoutMs = GetSfcTimeoutMs();
            bool sameAsDisplayed = string.Equals(before.Pid, pid, StringComparison.OrdinalIgnoreCase);
            var data = await WaitSfcResultAsync(pid, sameAsDisplayed, timeoutMs).ConfigureAwait(false);
            if (data == null)
            {
                SetQueueStatus($"⚠️ Timeout: {pid} (Sent to other SFC module)", Color.DarkOrange);
                return;
            }

            var guideMsg = _translator.TranslateAndGuide(data.Message);
            string uiMsg;
            if (string.IsNullOrWhiteSpace(guideMsg.Translated) || string.Equals(guideMsg.Translated, data.Message, StringComparison.OrdinalIgnoreCase))
            {
                uiMsg = data.Message;
            }
            else
            {
                uiMsg = $"{data.Message} / {guideMsg.Translated}";
            }

            if (Classify(data.Result) != ResultKind.Ok)
            {
                SetQueueStatus($"❌ NG: {data.PID} - {uiMsg}", Color.Red);
                await ShowScanAlertAsync("RESULTADO NG / NG RESULT", pid, BuildNgDetail(data)).ConfigureAwait(false);
                return;
            }

            string? mismatchEbr = await OnUiAsync<string?>(() => RegisterOkPid(data, uiMsg)).ConfigureAwait(false);
            if (mismatchEbr != null)
            {
                SetQueueStatus($"❌ EBR Mismatch: {data.PID}", Color.Red);
                await ShowScanAlertAsync("DESAJUSTE DE EBR / EBR MISMATCH", pid,
                    $"EBR en SFC '{data.EBR}' ≠ EBR configurado '{mismatchEbr}'.\r\n⚠️ ¡SEPARE esta tablilla!\r\n\r\n" +
                    $"SFC EBR '{data.EBR}' ≠ configured EBR '{mismatchEbr}'.\r\n⚠️ SEPARATE this board!").ConfigureAwait(false);
                return;
            }

            // Awaited (not fire-and-forget) so the PID is in DB before the label query reads the batch
            bool saved = false;
            for (int attempt = 1; attempt <= 3 && !saved; attempt++)
            {
                saved = await UploadToDb(data).ConfigureAwait(false);
                if (!saved) await Task.Delay(300 * attempt).ConfigureAwait(false);
            }

            if (!saved)
            {
                lock (_pendingLock) _pendingPids.Remove(data.PID);
                await OnUiAsync(() =>
                {
                    int c; lock (_pendingLock) c = _pendingPids.Count;
                    lbPreviewScanQty.Text = c.ToString();
                    return 0;
                }).ConfigureAwait(false);
                SetQueueStatus($"❌ Lỗi lưu DB: {data.PID}", Color.Red);
                await ShowScanAlertAsync("ERROR AL GUARDAR EN BD / DB SAVE FAILED", pid,
                    $"No se pudo guardar el PID en la base de datos (PID ya liberado en SFC). NO se contó.\r\n⚠️ Avise a ingeniería/QA. Error: {LastUploadError}\r\n\r\n" +
                    $"Could not save the PID to the database (already released in SFC). NOT counted.\r\n⚠️ Notify engineering/QA. Error: {LastUploadError}").ConfigureAwait(false);
                return;
            }

            if (await OnUiAsync<bool>(IsQuantityReached).ConfigureAwait(false))
                await RunOnUiAsync(PrintAndResetCoreAsync).ConfigureAwait(false);
        }

        private async Task<(string Title, string Detail)?> PrecheckAsync(string pid)
        {
            string savedEbr = await OnUiAsync<string>(() => txtSettingEBR.Text.Trim()).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(savedEbr))
            {
                string scannedEbr = pid.Length >= 11 ? pid[..11] : pid;
                if (!scannedEbr.Equals(savedEbr, StringComparison.OrdinalIgnoreCase))
                {
                    return ("DESAJUSTE DE EBR / EBR MISMATCH",
                        $"El EBR del PID '{scannedEbr}' es diferente al EBR guardado '{savedEbr}'.\r\n⚠️ ¡SEPARE esta tablilla antes de continuar!\r\n\r\n" +
                        $"The EBR of the PID '{scannedEbr}' is different from the saved EBR '{savedEbr}'.\r\n⚠️ SEPARATE this board before continuing!");
                }
            }

            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);

            if (!IsBlockCheckEnabled())
            {
                // Tự động gỡ block (Status = 0) để có thể lưu vào tb_scan_out bình thường
                await db.TbProductionBlocks
                    .Where(x => x.Pid == pid && x.Status == 1)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, 0))
                    .ConfigureAwait(false);
                return null;
            }

            var block = await db.TbProductionBlocks
                .FirstOrDefaultAsync(x => x.Pid == pid && x.Status == 1)
                .ConfigureAwait(false);
            if (block == null) return null;

            string reason = string.IsNullOrWhiteSpace(block.Reason) ? "Blocked by QA" : block.Reason;
            string station = $"{block.McType} {block.McId}".Trim();
            string stationText = string.IsNullOrEmpty(station) ? "" : $" ({station})";
            return ("TABLILLA BLOQUEADA / BLOCKED BOARD",
                $"El PID está BLOQUEADO por: {reason}{stationText}.\r\n⚠️ ¡SEPARE esta tablilla y entréguela a QA!\r\n\r\n" +
                $"The PID is BLOCKED due to: {reason}{stationText}.\r\n⚠️ SEPARATE this board and hand it over to QA!");
        }

        private bool IsBlockCheckEnabled()
        {
            string? raw = _config.GetSection("BlockCheckSetting")["EnableBlockCheck"];
            if (string.IsNullOrWhiteSpace(raw)) return true;
            return bool.TryParse(raw, out var enabled) && enabled;
        }

        private int GetSfcTimeoutMs() =>
            int.TryParse(_config.GetSection("PortSFC")["ResponseTimeoutMs"], out var ms) && ms > 0 ? ms : DefaultSfcTimeoutMs;

        private Task<(string Pid, string Result)> ReadSfcHeadAsync() =>
            _uiaWorker.Run(() =>
            {
                EnsureHandles();
                return (Normalize(_automationService2.SafeGetText(_pidEl)),
                        Normalize(_automationService2.SafeGetText(_resultEl)));
            });

        private async Task<PCB?> WaitSfcResultAsync(string pid, bool sameAsDisplayed, int timeoutMs)
        {
            var sw = Stopwatch.StartNew();
            string? candidate = null;
            long candidateSince = 0;

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                await Task.Delay(SfcPollMs).ConfigureAwait(false);
                var head = await ReadSfcHeadAsync().ConfigureAwait(false);

                bool isFinal = string.Equals(head.Pid, pid, StringComparison.OrdinalIgnoreCase)
                               && Classify(head.Result) != ResultKind.Other
                               && (!sameAsDisplayed || sw.ElapsedMilliseconds >= SamePidSettleMs);
                if (!isFinal)
                {
                    candidate = null;
                    continue;
                }

                if (!string.Equals(candidate, head.Result, StringComparison.Ordinal))
                {
                    candidate = head.Result;
                    candidateSince = sw.ElapsedMilliseconds;
                    continue;
                }

                if (sw.ElapsedMilliseconds - candidateSince >= ResultStableMs)
                    return await ReadSfcDetailAsync(head.Pid, head.Result).ConfigureAwait(false);
            }
            return null;
        }

        private async Task<PCB> ReadSfcDetailAsync(string pid, string result)
        {
            bool isNg = Classify(result) == ResultKind.Ng;
            var tail = await _uiaWorker.Run(() => (
                Ebr: Normalize(_automationService2.GetText(_config2.Controls.ModelSuffixUc, "txtText")),
                Wo: Normalize(_automationService2.GetText(_config2.Controls.WorkOrderUc, "txtText")),
                Prog: Normalize(_automationService2.GetText(_config2.Controls.ResultQty)),
                Msg: isNg ? Normalize(_automationService2.GetInnerMessageFromResult(_config2.Controls.ResultUc, _config2.Controls.ResultText)) : ""
            )).ConfigureAwait(false);

            var data = new PCB
            {
                PID = pid,
                Result = result,
                EBR = tail.Ebr,
                WO = tail.Wo,
                Progress = tail.Prog,
                Message = tail.Msg,
                Inspector1 = _inspector1,
                Inspector2 = _inspector2
            };

            // Already scanned-out boards count as OK (same rule as the screen watcher)
            if (isNg && tail.Msg.Contains("Already Shipped", StringComparison.OrdinalIgnoreCase))
            {
                data.Result = "OK";
                data.Message = pid;
            }
            return data;
        }

        /// <returns>null when the PID was counted; otherwise the configured EBR that did not match.</returns>
        private string? RegisterOkPid(PCB data, string translatedMsg)
        {
            string setting = txtSettingEBR.Text.Trim();
            string ebr = (data.EBR ?? string.Empty).Trim();
            if (setting.Length > 0 && ebr.Length > 0 && !ebr.Equals(setting, StringComparison.OrdinalIgnoreCase))
                return setting;

            if (setting.Length == 0 && ebr.Length > 0)
            {
                txtSettingEBR.Text = ebr;
                _settingEBR = ebr;
            }
            if (ebr.Length > 0) lbPreviewPno.Text = ebr;

            int count;
            lock (_pendingLock)
            {
                _pendingPids.Add(data.PID);
                count = _pendingPids.Count;
            }
            lbPreviewScanQty.Text = count.ToString();
            
            string displayMsg = string.IsNullOrWhiteSpace(translatedMsg) ? "OK" : translatedMsg;
            string combinedMsg = displayMsg.StartsWith(data.PID, StringComparison.OrdinalIgnoreCase)
                ? displayMsg
                : $"{data.PID} | {displayMsg}";

            lbDetailStatus.Text = $"✅ {combinedMsg} | Contados: {count} | Pendientes: {QueuedCount()}";
            lbDetailStatus.ForeColor = Color.Green;
            
            // Also update the main queue status label for better visibility
            SetQueueStatus($"✅ {combinedMsg}", Color.Green);
            return null;
        }

        private string BuildNgDetail(PCB data)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Resultado SFC / SFC result: {data.Result}");

            if (!string.IsNullOrWhiteSpace(data.Message))
            {
                var guide = _translator.TranslateAndGuide(data.Message);
                sb.AppendLine().AppendLine(data.Message);
                if (!string.Equals(guide.Translated, data.Message, StringComparison.OrdinalIgnoreCase))
                    sb.AppendLine(guide.Translated);
                if (!string.IsNullOrWhiteSpace(guide.ExplainEs))
                    sb.AppendLine().AppendLine($"★ {guide.ExplainEs}");
            }
            return sb.ToString();
        }

        private Task ShowScanAlertAsync(string title, string pid, string detail)
        {
            // Do NOT overwrite the translated message we just set in ProcessQueuedPidAsync, 
            // unless we want to show the title in the status bar too.
            // We'll leave the SetQueueStatus out so the translated NG message remains visible.
            return RunOnUiAsync(() =>
            {
                try { PlaySoundNg(); } catch { }

                // Keyboard-wedge scans typed into the popup are forwarded to the queue (popup is not closed by them)
                Action<char>? forward = _scannerMode.Equals("KeyboardHID", StringComparison.OrdinalIgnoreCase)
                    ? c => HandleScanChar(c)
                    : null;

                _alertOpen = true;
                try
                {
                    using var dlg = new frmScanAlert(title, pid, detail, forward);
                    dlg.Shown += (_, _) => ForceForeground(dlg);
                    dlg.ShowDialog(this);
                }
                finally
                {
                    _alertOpen = false;
                }

                ForceForeground(this);
                return Task.CompletedTask;
            });
        }

        private void SetQueueStatus(string text, Color color)
        {
            if (IsDisposed || !IsHandleCreated) return;
            BeginInvoke(new Action(() =>
            {
                lbDetailStatus.Text = text;
                lbDetailStatus.ForeColor = color;
            }));
        }

        private Task RunOnUiAsync(Func<Task> func)
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            if (IsDisposed || !IsHandleCreated)
            {
                tcs.SetResult();
                return tcs.Task;
            }

            BeginInvoke(new Action(async () =>
            {
                try
                {
                    await func();
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }
    }
}

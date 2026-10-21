using Microsoft.EntityFrameworkCore;
using MSFC.Data;
using MSFC.Dto;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSFC
{
    public partial class frmTraceAndPrint : Form
    {
        private readonly IDbContextFactory<MMesDbContext> _dbFactory;
        private PrintDocument printDoc = new PrintDocument();
        string _SettingEBR = string.Empty;
        private ManufacturingTagDto _tagLabel = new ManufacturingTagDto();
        private readonly object _pendingLock = new();
        private readonly HashSet<string> _pendingPids = new(StringComparer.OrdinalIgnoreCase);
        private string _ClientIp;
        public frmTraceAndPrint(IDbContextFactory<MMesDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
            InitializeComponent();
            try { this.Icon = Form1.GetAppIcon(); } catch { }
            string hostName = Dns.GetHostName();
            // Get IP of client
            _ClientIp = Dns.GetHostAddresses(hostName)
                           .FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                           ?.ToString() ?? DateTime.Now.ToString("ffff");
            setPlaceHlderText();
            SetupSettingButton();
        }

        private void SetupSettingButton()
        {
            try
            {
                Button btnSetting = new Button
                {
                    Text = "⚙️ Settings",
                    Size = new Size(110, 34),
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnSetting.FlatAppearance.BorderSize = 1;
                btnSetting.FlatAppearance.BorderColor = Color.DarkGray;

                btnSetting.Click += (s, e) =>
                {
                    using var pwdDlg = new frmAdminPassword();
                    if (pwdDlg.ShowDialog(this) == DialogResult.OK && pwdDlg.IsAuthenticated)
                    {
                        using var settingsDlg = new frmSettings();
                        settingsDlg.ShowDialog(this);
                    }
                };

                btnSetting.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                btnSetting.Location = new Point(10, this.ClientSize.Height - 42);
                this.Controls.Add(btnSetting);
                btnSetting.BringToFront();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SetupSettingButton Error] {ex.Message}");
            }
        }

        private void frmTraceAndPrint_Load(object sender, EventArgs e)
        {

        }
        private void setPlaceHlderText()
        {
            this.ActiveControl = txtScanPid;  // txtScan là TextBox của bạn
            txtScanPid.Clear();
            txtScanPid.Focus();
            //textBox1.PlaceholderText = "Escanee la etiqueta aquí para verificar el EBR...";
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {

        }
        string Normalize(string s)
        {
            // 1. Nếu null, rỗng, toàn khoảng trắng → trả về chuỗi rỗng luôn
            if (string.IsNullOrWhiteSpace(s))
                return "";

            // 2. Chuẩn hoá Unicode sang FormKC (Compatibility Composition)
            //    - Ghép lại các ký tự tách rời (O + ́ → Ó)
            //    - Biến các ký tự fullwidth/compatibility thành bản ASCII (ＯＫ → OK)
            var norm = s.Normalize(NormalizationForm.FormKC);

            // 3. Loại bỏ ký tự "khó chịu"
            var sb = new StringBuilder(norm.Length);
            foreach (var ch in norm)
            {
                if (char.IsControl(ch)) continue;     // bỏ ký tự điều khiển (CR, LF, NULL…)
                //if (char.IsPunctuation(ch)) continue; // bỏ dấu câu/ký tự đặc biệt (. , ! ? …)
                //if (char.IsSymbol(ch)) continue;      // bỏ ký tự dạng symbol (©, ±, ÷, …)
                //if (char.IsPunctuation(ch) && ch != '/') continue; // bỏ dấu câu trừ khi là '/'
                sb.Append(ch);                        // giữ lại chữ cái, số, khoảng trắng bình thường
            }

            // 4. Gom nhiều khoảng trắng liên tiếp thành 1 và Trim 2 đầu
            var cleaned = Regex.Replace(sb.ToString(), @"\s+", " ").Trim();

            // 5. Đưa toàn bộ về in hoa invariant (không phụ thuộc locale)
            return cleaned.ToUpperInvariant();
        }
        private void ApplyStatusStyle(bool kind)
        {

            // Cho label trong suốt để nhìn màu panel
            lbStatus.BackColor = Color.Transparent;

            switch (kind)
            {
                case true:
                    //pnStatus.BackColor = Color.Lime;
                    lbStatus.ForeColor = Color.Black;
                    lbStatus.BackColor = Color.Lime;
                    rtxtDetailExplain.Text = "";

                    break;
                default:
                    //pnStatus.BackColor = Color.Red;
                    lbStatus.ForeColor = Color.Black;
                    lbStatus.BackColor = Color.Red;

                    rtxtDetailExplain.ForeColor = Color.Red;
                    rtxtDetailExplain.BackColor = Color.Yellow;
                    rtxtDetailExplain.Font = new Font(rtxtDetailExplain.Font.FontFamily, 26, FontStyle.Bold);
                    break;
            }
        }
        private void ShowNotice(string msg, bool kind)
        {
            if (kind)
            {
                ApplyStatusStyle(true);
                lbStatus.Text = "OK";
               
                rtxtDetailExplain.Text = "";
            }
            else
            {
                ApplyStatusStyle(false);
                lbStatus.Text = "NG";
                rtxtDetailExplain.Text = $"\u2605 {msg}";
            }

        }
        private async void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter || e.KeyChar == '\r' || e.KeyChar == '\n')
                {
                    string scanned = Normalize(txtScanPid.Text.Trim());
                    setPlaceHlderText();
                    if (!string.IsNullOrEmpty(scanned))
                    {
                        // Lấy thông tin từ db
                        using var db = await _dbFactory.CreateDbContextAsync();

                        var data = await db.TbScanOuts
                          .Where(x => x.Pid == scanned).FirstOrDefaultAsync();
                        if (data == null)
                        {
                            var msg = $"Este código S/N debe escanearse en la aplicación principal.";
                            //var msg = $"No data.";
                            ShowNotice(msg, false);
                            return;
                        }
                        if (string.IsNullOrWhiteSpace(_SettingEBR))
                        {
                            _SettingEBR = data.ModelSuffix;
                            UpdateUI(_SettingEBR);
                        }
                        if (!data.PartNo.Equals(_SettingEBR, StringComparison.OrdinalIgnoreCase))
                        {
                            var msg = $"⚠️ P/No escaneado ({data.PartNo}) no coincide con P/No configurado ({_SettingEBR}).";
                            //var msg = $"Wrong PN.";
                            ShowNotice(msg, false);
                            return;
                        }
                        if (data.TagId.HasValue)
                        {
                            var msg = $"Este código S/N [{data.Pid}] debe estar estampado con la etiqueta {data.TagId} en la fecha {data.PrintDate?.ToString("dd/MM/yyyy")}";
                            //var msg = $"Scanned at other Tag.";
                            ShowNotice(msg, false);
                            return;
                        }
                        if (!_pendingPids.Contains(scanned))
                        {
                            lock (_pendingLock)
                            {
                                _pendingPids.Add(scanned);
                            }
                            lstScanedPids.Items.Add(scanned);
                        }
                        ShowNotice(scanned, true);
                        //

                    }

                    e.Handled = true; // Ngăn tiếng "ding" khi nhấn Enter
                }
            }
            catch (Exception ex)
            {

                ShowNotice(ex.Message, false);
            }
            // Nếu scanner gửi Enter (\r hoặc \n)

        }
        #region Print Setting & Generate label tag
        // offset cấu hình (mm). dương = dịch xuống / dịch sang phải
        float _offsetXmm = 0.0f;
        float _offsetYmm = -2.0f;
        float _labelWidthMm = 55.0f;
        float _labelHeightMm = 45.0f;
        float _topMarginMm = 2.0f;

        // đổi mm -> hundredths of an inch (đơn vị của MarginBounds)
        static int MmToHundredths(float mm) => (int)Math.Round(mm / 25.4f * 100f);

        void PrintTag(ManufacturingTagDto tag)
        {
            try
            {
                AddLog("PrintTag: Start printing tag.");
                var doc = new PrintDocument();
                doc.PrinterSettings = new PrinterSettings();

                // Chuyển đổi mm sang đơn vị máy in
                int w = MmToHundredths(_labelWidthMm);
                int h = MmToHundredths(_labelHeightMm);

                // Tìm khổ giấy khớp
                var ps = doc.PrinterSettings.PaperSizes
                    .Cast<PaperSize>()
                    .FirstOrDefault(p =>
                        (Math.Abs(p.Width - w) < 15 && Math.Abs(p.Height - h) < 15) ||
                        (Math.Abs(p.Width - h) < 15 && Math.Abs(p.Height - w) < 15)
                    );

                if (ps != null)
                    doc.DefaultPageSettings.PaperSize = ps;
                else
                    doc.DefaultPageSettings.PaperSize = new PaperSize($"Custom {_labelWidthMm}x{_labelHeightMm}", w, h);

                doc.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                doc.OriginAtMargins = true;              // (0,0) = MarginBounds.TopLeft
                doc.DefaultPageSettings.Landscape = false; // nếu bị xoay hãy thử true
                AddLog("PrintTag: Start generate label...");
                _tagLabel = tag;
                doc.PrintPage += GenerateLabel_FitMarginBounds;

                AddLog("PrintTag: Calling doc.Print()...");
                doc.Print();
                AddLog("PrintTag: Print completed.");
            }
            catch (Exception ex)
            {
                AddLog($"PrintTag: Exception - {ex.Message}\r\n{ex.StackTrace}");
                throw;
            }
        }

        private void AddLog(string msg)
        {
        }
        void GenerateLabel_FitMarginBounds(object sender, PrintPageEventArgs e)
        {
            if (_tagLabel == null) { e.Cancel = true; return; }
            var g = e.Graphics;

            try
            {
                Rectangle deviceBounds = e.MarginBounds;
                if (deviceBounds.Width <= 0 || deviceBounds.Height <= 0)
                {
                    deviceBounds = new Rectangle(e.PageBounds.Left + 10, e.PageBounds.Top + 10,
                                                 Math.Min(400, e.PageBounds.Width - 20),
                                                 Math.Min(300, e.PageBounds.Height - 20));
                }

                // apply offsets (in hundredths of inch)
                int offX = MmToHundredths(_offsetXmm);
                int offY = MmToHundredths(_offsetYmm);
                var dest = new Rectangle(deviceBounds.Left + offX, deviceBounds.Top + offY, deviceBounds.Width, deviceBounds.Height);

                if (dest.Width <= 0 || dest.Height <= 0)
                {
                    e.HasMorePages = false;
                    return;
                }

                // Thiết lập kích thước thiết kế theo biến cấu hình
                float designW = MmToHundredths(_labelWidthMm);
                float designH = MmToHundredths(_labelHeightMm);

                // Compute key layout values in design units (same logic as DrawTag_DesignUnits)
                float padY = designH * 0.015f;            // top/bottom pad
                float bottomOfQty = designH - padY;

                // small extra gap after content in mm
                float extraGapMm = 0.1f;
                int extraGapDeviceUnits = MmToHundredths(extraGapMm);

                // Tính scale dựa trên chiều rộng
                float scaleBasedOnWidth = dest.Width / designW;

                // Chiều cao thiết bị yêu cầu để vẽ hết nội dung:
                float requiredDeviceHeight = bottomOfQty * scaleBasedOnWidth + extraGapDeviceUnits;

                // Nếu thiết bị in là giấy liên tục (continuous paper - dest.Height rất lớn)
                float continuousThresholdMultiplier = 3.0f;
                if (dest.Height > designH * continuousThresholdMultiplier)
                {
                    int minAllowed = MmToHundredths(10); // Tối thiểu 10mm
                    int clampH = Math.Max(minAllowed, (int)Math.Ceiling(requiredDeviceHeight));
                    dest = new Rectangle(dest.Left, dest.Top, dest.Width, Math.Min(dest.Height, clampH));

                    float scaleYIfNeeded = (float)dest.Height / designH;
                    float scale = Math.Min(scaleBasedOnWidth, scaleYIfNeeded);

                    float drawnW = designW * scale;

                    float translateX = dest.Left + (dest.Width - drawnW) / 2f;
                    float topMarginUnit = MmToHundredths(_topMarginMm);
                    float offsetUnit    = MmToHundredths(_offsetYmm);
                    float translateY = dest.Top + topMarginUnit + offsetUnit;

                    var state = g.Save();
                    try
                    {
                        g.TranslateTransform(translateX, translateY);
                        g.ScaleTransform(scale, scale);

                        DrawTag_DesignUnits(g, designW, designH, _tagLabel);
                    }
                    finally
                    {
                        g.Restore(state);
                    }

                    e.HasMorePages = false;
                    return;
                }

                // Khổ giấy thông thường
                float scaleX = dest.Width / designW;
                float scaleY = dest.Height / designH;
                float finalScale = Math.Min(scaleX, scaleY);
                if (finalScale > 1.0f) finalScale = 1.0f;

                float finalDrawnW = designW * finalScale;

                float translateXNormal = dest.Left + (dest.Width - finalDrawnW) / 2f;
                float topMarginUnitNormal = MmToHundredths(_topMarginMm);
                float offsetUnitNormal    = MmToHundredths(_offsetYmm);
                float translateYNormal = dest.Top + topMarginUnitNormal + offsetUnitNormal;

                var stateNormal = g.Save();
                try
                {
                    g.TranslateTransform(translateXNormal, translateYNormal);
                    g.ScaleTransform(finalScale, finalScale);

                    DrawTag_DesignUnits(g, designW, designH, _tagLabel);
                }
                finally
                {
                    g.Restore(stateNormal);
                }

                e.HasMorePages = false;
            }
            catch (Exception ex)
            {
                AddLog("[ERROR] GenerateLabel_FitMarginBounds Exception: " + ex.ToString());
                e.HasMorePages = false;
            }
        }

        void DrawTag_DesignUnits(Graphics g, float W, float H, ManufacturingTagDto tag)
        {
            using var pen = new Pen(Color.Black, 1);

            // 1. VẼ VIỀN
            g.DrawRectangle(pen, 0, 0, W - 1, H - 1);

            // 2. Padding nhỏ (1.5%)
            float padX = W * 0.015f;
            float padY = H * 0.015f;

            using var fTitle = new Font("Segoe UI", 11, FontStyle.Bold, GraphicsUnit.Point);
            using var fBody  = new Font("Arial Narrow", 9, FontStyle.Bold, GraphicsUnit.Point);

            // 3. TITLE AREA (~16% chiều cao)
            float titleH    = H * 0.16f;
            var   titleRect = new RectangleF(padX, padY, W - padX * 2, titleH);
            var   sfn       = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

            g.DrawString("Manufacture Tag", fTitle, Brushes.Black, titleRect, sfn);

            float separatorY = titleRect.Bottom;
            g.DrawLine(pen, 0, separatorY, W, separatorY);

            // 4. BODY CONTENT
            float bodyY = separatorY + 1.0f;
            float bodyH = (H - padY) - bodyY;
            float lineH = bodyH / 6.0f; // 6 dòng

            // QR zone: bên phải label, góc dưới cùng
            float qrSize = bodyH * 0.45f;                       // Giảm QR xuống 45% chiều cao body
            float qrGap  = W * 0.015f;                          // khoảng cách giữa text và QR
            float qrX    = W - padX - qrSize - (W * 0.30f);     // dịch trái 30% chiều rộng nhãn để sát gần Ins1, Ins2
            float qrY    = bodyY + bodyH - qrSize;              // đẩy QR xuống đáy của body (căn dưới)
            float textW_narrow  = qrX - qrGap - padX;           // text thu hẹp, không chồng QR
            float textW_full    = W - padX * 2;                 // text full chiều ngang

            void DrawLine(string text, Font font, int index)
            {
                float y    = bodyY + (index * lineH);
                float currentTextW = (y + lineH * 0.8f > qrY) ? textW_narrow : textW_full;
                var   rect = new RectangleF(padX, y, currentTextW, lineH);
                DrawFitText(g, text, font, Brushes.Black, rect, 4.5f);
            }

            DrawLine($"{tag?.ModelName ?? ""}",                 fBody, 0);
            DrawLine($"Date: {tag?.Date ?? ""}",                fBody, 1);
            DrawLine($"Part No: {FormatPartNo(tag?.PartNo)}",   fBody, 2);
            DrawLine($"Q'ty: {tag?.Quantity}",                  fBody, 3);
            DrawLine($"Ins 1: {tag?.Inspector1 ?? ""}",   fBody, 4);
            DrawLine($"Ins 2: {tag?.Inspector2 ?? ""}",   fBody, 5);

            // 5. QR CODE — nội dung = Part No (EBR)
            string qrContent = tag?.PartNo ?? "";
            if (!string.IsNullOrEmpty(qrContent))
            {
                try
                {
                    using var qrGenerator = new QRCodeGenerator();
                    using var qrData      = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.M);
                    using var qrCode      = new QRCode(qrData);

                    // FIX: dùng pixel thực (10px/module) thay vì design units
                    // → đảm bảo bitmap đủ độ phân giải trước khi scale vào qrRect
                    int pixelPerModule = 10;
                    using Bitmap qrBitmap = qrCode.GetGraphic(pixelPerModule, Color.Black, Color.White, drawQuietZones: true);

                    var qrRect = new RectangleF(qrX, qrY, qrSize, qrSize);
                    g.DrawImage(qrBitmap, qrRect);
                }
                catch (Exception ex)
                {
                    // Log lỗi QR để debug — label vẫn in bình thường
                    System.Diagnostics.Debug.WriteLine($"[QR ERROR] {ex.GetType().Name}: {ex.Message}");
                    AddLog($"[QR ERROR] {ex.GetType().Name}: {ex.Message}");
                }
            }
        }

        string FormatPartNo(string? partNo)
        {
            if (string.IsNullOrWhiteSpace(partNo) || partNo.Length < 11)
                return partNo ?? "";

            // Ví dụ: EBR12345678 → EBR 1234 5678
            string prefix = partNo.Substring(0, 3);
            string mid    = partNo.Substring(3, 4);
            string end    = partNo.Substring(7, 4);

            return $"{prefix} {mid} {end}";
        }

        /// <summary>
        /// Vẽ text vào khung và TỰ GIẢM cỡ chữ (Point) để vừa chiều cao.
        /// Không wrap. Nếu vẫn không vừa ở cỡ tối thiểu -> vẽ với ellipsis.
        /// </summary>
        /// <param name="g">Graphics hiện tại</param>
        /// <param name="text">Chuỗi cần vẽ</param>
        /// <param name="baseFont">Font gốc (lấy họ chữ & style)</param>
        /// <param name="brush">Màu bút</param>
        /// <param name="rect">Khung cần vẽ</param>
        /// <param name="minPointSize">Cỡ chữ nhỏ nhất cho phép</param>
        static void DrawFitText(Graphics g, string text, Font baseFont, Brush brush, RectangleF rect, float minPointSize = 6.0f)
        {
            // Không xuống dòng; nếu dài quá sẽ xử lý bằng cách giảm cỡ chữ/ellipsis
            var fmt = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            // Thử giảm dần từ cỡ hiện tại đến min
            for (float sz = baseFont.Size; sz >= minPointSize; sz -= 0.5f)
            {
                using var f = new Font(baseFont.FontFamily, sz, baseFont.Style, GraphicsUnit.Point);
                // MeasureString với width giới hạn để ước lượng có vừa chiều cao không
                var measured = g.MeasureString(text, f, (int)Math.Ceiling(rect.Width), fmt);

                if (measured.Height <= rect.Height + 0.5f)
                {
                    g.DrawString(text, f, brush, rect, fmt);
                    return;
                }
            }

            // Fallback: cỡ min + ellipsis
            using var fMin = new Font(baseFont.FontFamily, minPointSize, baseFont.Style, GraphicsUnit.Point);
            g.DrawString(text, fMin, brush, rect, fmt);
        }
        string IpToNumericString(string ip)
        {
            return string.Join("", ip.Split('.').Select(b => int.Parse(b).ToString("D3")));
        }
        long GenerateTagId()
        {
            DateTime now = DateTime.Now;

            string ipNum = IpToNumericString(_ClientIp);
            // timestamp yyMMddHHmmssffff
            string timestamp = now.ToString("yyMMddHHmmssfff");
            var last6Digits = ipNum.Substring(ipNum.Length - 3);
            // Ghép lại
            string tagIdStr = last6Digits + timestamp;
            long tagIdValue = long.Parse(tagIdStr);

            return tagIdValue;
        }
        private async void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!lbStatus.Text.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    var msg = "El último PID escaneado debe ser OK y coincidir con EBR.";
                    //var msg = $"Last PID scan must be OK";
                    ShowNotice(msg, false);
                    txtScanPid.Focus();
                    return;
                }

                // snapshot danh sách PID đang chờ in (batch hiện tại)
                string[] batchPids;
                lock (_pendingLock)
                {
                    batchPids = _pendingPids.ToArray();
                }

                if (batchPids.Length == 0)
                {
                    var msg = "No hay PIDs pendientes para imprimir.R.";
                    //var msg = $"No pid list";
                    ShowNotice(msg, false);
                    txtScanPid.Focus();
                    return;
                }

                //AddLog("Start query (batch only)");

                using var db = await _dbFactory.CreateDbContextAsync();

                // Lấy đúng các dòng thuộc batch hiện tại, chưa in
                //var tagDatas = await db.TbScanOuts
                //    .Where(x => x.PartNo == _SettingEBR
                //                && !x.TagId.HasValue
                //                && batchPids.Contains(x.Pid))
                //    .ToListAsync();
                var tagDatas = await (
                     from s in db.TbScanOuts
                     join m in db.TbModelDicts on s.PartNo equals m.PartNo into gj
                     from m in gj.DefaultIfEmpty()     // <-- left join
                     where batchPids.Contains(s.Pid)
                     select new
                     {
                         PartNo = m != null ? m.PartNo : null,   // có thể null nếu không match
                         ModelName = m != null ? m.ModelName : null,
                         Board = m != null ? m.Board : null,
                         Pid = s.Pid
                     }
                 ).ToListAsync();
                if (tagDatas.Count == 0)
                {
                    ShowNotice("No se encontraron PIDs válidos en el lote actual.", false);
                    txtScanPid.Focus();
                    //// loại các PID không còn match khỏi batch để tránh lặp vô hạn
                    //lock (_pendingLock)
                    //{
                    //    foreach (var pid in batchPids) _pendingPids.Remove(pid);
                    //}

                    return;
                }

                //AddLog("Gen TagID");
                var now = DateTime.Now;
                long tagId = GenerateTagId();

                //AddLog("Assign data for tag label");
                _tagLabel = new ManufacturingTagDto
                {
                    ModelName = $"{tagDatas[0].ModelName}-{tagDatas[0].Board}",
                    Date = now.ToString("dd/MM/yyyy HH:mm:ss"),
                    PartNo = tagDatas[0].PartNo,
                    Quantity = tagDatas.Count,                // ✅ chỉ đếm batch hiện tại
                    Process = "",
                    TagId = tagId,
                    PidList = tagDatas.Select(x => x.Pid).ToList()
                };

                //AddLog("Print");
                PrintTag(_tagLabel);

                //AddLog("Update DB");
                await db.TbScanOuts
                    .Where(x => _tagLabel.PidList.Contains(x.Pid))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(r => r.TagId, r => tagId)
                        .SetProperty(r => r.PrintAt, r => DateTime.Now)
                        .SetProperty(r => r.PrintDate, r => DateOnly.FromDateTime(DateTime.Now))
                    );

                // Xóa đúng các PID vừa in khỏi batch
                lock (_pendingLock)
                {
                    foreach (var pid in _tagLabel.PidList)
                        _pendingPids.Remove(pid);
                }

                // Reset biến/UI
                _tagLabel = new ManufacturingTagDto();
                lstScanedPids.Items.Clear();
                UpdateUI("");
                txtScanPid.Focus();
            }
            catch (Exception ex)
            {
                ShowNotice(ex.Message, false);
            }
        }
        private void UpdateUI(string settingEBR)
        {
            var msg = $"Esta es la interfaz para revisar el EBR e imprimir la etiqueta. ({settingEBR})";
            lbModelInfor.Text = msg;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _tagLabel = new ManufacturingTagDto();
            lstScanedPids.Items.Clear();
            UpdateUI("");
            lock (_pendingLock)
            {
                foreach (var pid in _tagLabel.PidList)
                    _pendingPids.Remove(pid);
            }
            _SettingEBR = string.Empty;

            pnStatus.BackColor = Color.Black;
            lbStatus.ForeColor = Color.Yellow;
            lbStatus.BackColor = Color.Black;
            lbStatus.Text = "LISTA";
            rtxtDetailExplain.Text = "";
            txtScanPid.Focus();
        }
    }
}

#endregion
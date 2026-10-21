using Microsoft.EntityFrameworkCore;
using MSFC.Data;
using MSFC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MSFC.UC
{
    public partial class frmXrayScanSum : Form
    {
        private readonly IDbContextFactory<MMesDbContext> _dbFactory;
        private readonly string _clientIp;
        private readonly List<string> _pids = new List<string>();

        private readonly string _logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "XRay_PID_Log.txt");
        private readonly string _inspectorFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "XRay_Inspector_Setting.txt");

        public frmXrayScanSum(IDbContextFactory<MMesDbContext> dbFactory, string clientIp)
        {
            _dbFactory = dbFactory;
            _clientIp = clientIp ?? "127.0.0.1";

            InitializeComponent();
            try { this.Icon = Form1.GetAppIcon(); } catch { }
        }

        private void frmXrayScanSum_Load(object sender, EventArgs e)
        {
            lnkLogFilePath.Text = Path.GetFileName(_logFilePath);
            ToolTip tip = new ToolTip();
            tip.SetToolTip(lnkLogFilePath, _logFilePath);

            // Load saved Inspector ID / Name if exists
            try
            {
                if (File.Exists(_inspectorFilePath))
                {
                    txtInspectorInput.Text = File.ReadAllText(_inspectorFilePath).Trim();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Load Inspector Setting Error] {ex.Message}");
            }

            UpdateSlotsUI();
            txtPidInput.Focus();
        }

        private void txtInspectorInput_TextChanged(object sender, EventArgs e)
        {
            picPreview.Invalidate();
        }

        private void btnSaveInspector_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(_logDir))
                {
                    Directory.CreateDirectory(_logDir);
                }
                string inspectorVal = txtInspectorInput.Text?.Trim() ?? "";
                File.WriteAllText(_inspectorFilePath, inspectorVal);
                MessageBox.Show("Inspector ID / Name saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                picPreview.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save Inspector setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPidInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ProcessAddPid();
            }
        }

        private void btnAddPid_Click(object sender, EventArgs e)
        {
            ProcessAddPid();
        }

        private void ProcessAddPid()
        {
            string rawInput = txtPidInput.Text?.Trim();
            if (string.IsNullOrEmpty(rawInput)) return;

            // RULE: Extract the last 11 characters of the scanned string
            string input = rawInput.Length > 11 ? rawInput.Substring(rawInput.Length - 11) : rawInput;

            // 1. Check max 6 PIDs
            if (_pids.Count >= 6)
            {
                MessageBox.Show("All 6 PIDs scanned! Please click 'Print Label' or 'Reset' to start a new batch.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPidInput.SelectAll();
                txtPidInput.Focus();
                return;
            }

            // 2. Prevent Duplicate PID in current batch
            if (_pids.Any(p => p.Equals(input, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"⚠️ DUPLICATE PID WARNING!\n\nPID '{input}' has already been scanned in this batch.", 
                                "Duplicate PID Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPidInput.SelectAll();
                txtPidInput.Focus();
                return;
            }

            // 3. Check Duplicate PID in Database History
            try
            {
                using var db = _dbFactory.CreateDbContext();
                bool existsInDb = db.TbXrayScanLogs.Any(x => 
                    x.Pid1 == input || x.Pid2 == input || x.Pid3 == input || 
                    x.Pid4 == input || x.Pid5 == input || x.Pid6 == input);

                if (existsInDb)
                {
                    var result = MessageBox.Show($"⚠️ PID ALREADY EXISTS IN DATABASE!\n\nPID '{input}' has been scanned previously in history.\nDo you still want to proceed?", 
                                                 "PID Exists in DB", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                    {
                        txtPidInput.SelectAll();
                        txtPidInput.Focus();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Check Duplicate DB Error] {ex.Message}");
            }

            // Add valid 11-character PID
            _pids.Add(input);

            // LOG ALL PIDs TO TEXT FILE
            AppendPidToTextLog(input);

            txtPidInput.Clear();
            txtPidInput.Focus();

            UpdateSlotsUI();
            picPreview.Invalidate();

            // When 6th PID is scanned: IMMEDIATELY SAVE & PRINT WITHOUT ANY POPUPS
            if (_pids.Count == 6)
            {
                SaveScanToDb();
                ExecutePrint();
            }
        }

        private void AppendPidToTextLog(string pid)
        {
            try
            {
                if (!Directory.Exists(_logDir))
                {
                    Directory.CreateDirectory(_logDir);
                }
                File.AppendAllText(_logFilePath, pid + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AppendPidToTextLog Error] {ex.Message}");
            }
        }

        private void lnkLogFilePath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLogFile();
        }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            OpenLogFile();
        }

        private void OpenLogFile()
        {
            try
            {
                if (!Directory.Exists(_logDir))
                {
                    Directory.CreateDirectory(_logDir);
                }
                if (!File.Exists(_logFilePath))
                {
                    File.WriteAllText(_logFilePath, "");
                }
                Process.Start(new ProcessStartInfo(_logFilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open log file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSlotsUI()
        {
            Label[] slots = new Label[] { lblSlot1, lblSlot2, lblSlot3, lblSlot4, lblSlot5, lblSlot6 };
            for (int i = 0; i < 6; i++)
            {
                if (i < _pids.Count)
                {
                    slots[i].Text = $"{i + 1}.  {_pids[i]}";
                    slots[i].ForeColor = Color.DarkGreen;
                    slots[i].BackColor = Color.LightGreen;
                }
                else
                {
                    slots[i].Text = $"{i + 1}.  [Not Scanned]";
                    slots[i].ForeColor = Color.Gray;
                    slots[i].BackColor = Color.WhiteSmoke;
                }
            }

            lblStatus.Text = $"Progress: {_pids.Count} / 6 PIDs";
            if (_pids.Count == 6)
            {
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text += " (COMPLETED - PRINTING...)";
            }
            else
            {
                lblStatus.ForeColor = Color.DarkSlateGray;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetData();
        }

        private void ResetData()
        {
            _pids.Clear();
            txtPidInput.Clear();
            UpdateSlotsUI();
            picPreview.Invalidate();
            txtPidInput.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_pids.Count == 0)
            {
                MessageBox.Show("No PID scanned yet to print!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveScanToDb();
            ExecutePrint();
        }

        private void SaveScanToDb()
        {
            try
            {
                using var db = _dbFactory.CreateDbContext();
                var log = new TbXrayScanLog
                {
                    BatchCode = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                    Pid1 = _pids.Count > 0 ? _pids[0] : null,
                    Pid2 = _pids.Count > 1 ? _pids[1] : null,
                    Pid3 = _pids.Count > 2 ? _pids[2] : null,
                    Pid4 = _pids.Count > 3 ? _pids[3] : null,
                    Pid5 = _pids.Count > 4 ? _pids[4] : null,
                    Pid6 = _pids.Count > 5 ? _pids[5] : null,
                    ScanAt = DateTime.Now,
                    ClientIp = _clientIp,
                    PrintedAt = DateTime.Now
                };
                db.TbXrayScanLogs.Add(log);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SaveScanToDb Error] {ex.Message}");
            }
        }

        private void ExecutePrint()
        {
            try
            {
                var doc = new PrintDocument();
                doc.PrintController = new StandardPrintController();

                // Physical vertical paper size (Width 50mm x Height 90mm)
                int paperWidthHundredths = (int)(50 * 3.93701);
                int paperHeightHundredths = (int)(90 * 3.93701);

                doc.DefaultPageSettings.PaperSize = new PaperSize("Custom X-Ray Vertical Label", paperWidthHundredths, paperHeightHundredths);
                doc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                doc.OriginAtMargins = true;

                doc.PrintPage += (s, ev) =>
                {
                    RenderLabelGraphic(ev.Graphics, ev.MarginBounds.Width, ev.MarginBounds.Height);
                    ev.HasMorePages = false;
                };

                // IMMEDIATE PRINTING WITHOUT ANY POPUP NOTIFICATIONS
                doc.Print();

                // AUTOMATICALLY RESET DATA IMMEDIATELY AFTER PRINTING
                ResetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Printing error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void picPreview_Paint(object sender, PaintEventArgs e)
        {
            RenderLabelGraphic(e.Graphics, picPreview.Width, picPreview.Height);
        }

        private void RenderLabelGraphic(Graphics g, float w, float h)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Background white
            g.Clear(Color.White);

            using var borderPen = new Pen(Color.Black, 2.5f);
            using var linePen = new Pen(Color.Black, 1.5f);

            using var fontHaengsung = new Font("Arial", 12f, FontStyle.Bold);
            using var fontTitle = new Font("Arial", 11.5f, FontStyle.Bold | FontStyle.Underline);
            using var fontHeader = new Font("Arial", 11f, FontStyle.Bold);
            using var fontPidNum = new Font("Arial", 10.5f, FontStyle.Bold);
            using var fontPidVal = new Font("Consolas", 10.5f, FontStyle.Bold);
            using var fontFooter = new Font("Arial", 10f, FontStyle.Bold);

            float padX = 12f;
            float padY = 12f;
            float rectW = w - (padX * 2);
            float rectH = h - (padY * 2);

            // 1. Outer Border
            g.DrawRectangle(borderPen, padX, padY, rectW, rectH);

            float curY = padY + 10f;
            float contentX = padX + 10f;

            // 2. Text "HAENG SUNG" at top right
            SizeF hsSize = g.MeasureString("HAENG SUNG", fontHaengsung);
            float hsX = padX + rectW - hsSize.Width - 10f;
            g.DrawString("HAENG SUNG", fontHaengsung, Brushes.Black, new PointF(hsX, curY));

            curY += hsSize.Height + 6f;

            // 3. Bold Underlined Title: "X-ray Inspection OK"
            g.DrawString("X-ray Inspection OK", fontTitle, Brushes.Black, new PointF(contentX, curY));
            curY += 34f;

            // Line separator
            g.DrawLine(linePen, padX, curY, padX + rectW, curY);
            curY += 10f;

            // 4. PID Section
            g.DrawString("PID:", fontHeader, Brushes.Black, new PointF(contentX, curY));
            curY += 26f;

            // 5. 6 PID Rows (1 to 6)
            float availableH = rectH - (curY - padY) - 75f;
            float rowH = availableH / 6.0f;
            if (rowH < 24f) rowH = 24f;

            for (int i = 0; i < 6; i++)
            {
                string pidValue = i < _pids.Count ? _pids[i] : "";
                string numStr = $"{i + 1}.";

                // Draw "1.", "2.", ...
                g.DrawString(numStr, fontPidNum, Brushes.Black, new PointF(contentX, curY));

                // Draw PID value
                if (!string.IsNullOrEmpty(pidValue))
                {
                    g.DrawString(pidValue, fontPidVal, Brushes.Navy, new PointF(contentX + 28f, curY));
                }

                // Underline line for PID
                float lineY = curY + 20f;
                g.DrawLine(linePen, contentX + 25f, lineY, padX + rectW - 10f, lineY);

                curY += rowH;
            }

            curY += 6f;
            // Line separator
            g.DrawLine(linePen, padX, curY, padX + rectW, curY);
            curY += 10f;

            // 6. FECHA : <current date>
            string fechaVal = DateTime.Now.ToString("dd-M-yy");
            g.DrawString($"FECHA :  {fechaVal}", fontFooter, Brushes.Black, new PointF(contentX, curY));
            float fechaLineY = curY + 20f;
            g.DrawLine(linePen, contentX + 68f, fechaLineY, padX + rectW - 10f, fechaLineY);

            curY += 32f;

            // 7. INSPECTOR : <saved/typed Inspector value>
            string inspectorVal = txtInspectorInput.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(inspectorVal))
            {
                g.DrawString($"INSPECTOR :  {inspectorVal}", fontFooter, Brushes.Black, new PointF(contentX, curY));
            }
            else
            {
                g.DrawString("INSPECTOR :", fontFooter, Brushes.Black, new PointF(contentX, curY));
            }
            float inspLineY = curY + 20f;
            g.DrawLine(linePen, contentX + 92f, inspLineY, padX + rectW - 10f, inspLineY);
        }
    }
}

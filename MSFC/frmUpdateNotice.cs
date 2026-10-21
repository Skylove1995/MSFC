using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MSFC.Dto;
using MSFC.Service;

namespace MSFC
{
    public partial class frmUpdateNotice : Form
    {
        private readonly UpdateCheckResponse _updateInfo;
        private readonly UpdateService _updateService;

        private Label lblTitle = null!;
        private Label lblVersionInfo = null!;
        private Label lblFileSize = null!;
        private RichTextBox rtbNotes = null!;
        private ProgressBar progressBar = null!;
        private Label lblStatus = null!;
        private Button btnUpdateNow = null!;
        private Button btnClose = null!;
        private CancellationTokenSource? _cts;

        public frmUpdateNotice(UpdateCheckResponse updateInfo)
        {
            _updateInfo = updateInfo;
            _updateService = new UpdateService();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "New Version Available - MSFC";
            this.Size = new Size(520, 430);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(32, 32, 36);
            this.ForeColor = Color.White;

            lblTitle = new Label
            {
                Text = "🚀 NEW UPDATE AVAILABLE!",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 180, 255),
                Location = new Point(20, 15),
                AutoSize = true
            };

            lblVersionInfo = new Label
            {
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gainsboro,
                Location = new Point(20, 48),
                AutoSize = true
            };

            lblFileSize = new Label
            {
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.Gray,
                Location = new Point(20, 72),
                AutoSize = true
            };

            Label lblNotesHeader = new Label
            {
                Text = "Release Notes:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 100),
                AutoSize = true
            };

            rtbNotes = new RichTextBox
            {
                Location = new Point(20, 125),
                Size = new Size(464, 170),
                BackColor = Color.FromArgb(42, 42, 48),
                ForeColor = Color.Gainsboro,
                Font = new Font("Consolas", 9.5F),
                ReadOnly = true,
                BorderStyle = BorderStyle.None
            };

            progressBar = new ProgressBar
            {
                Location = new Point(20, 310),
                Size = new Size(464, 18),
                Visible = false
            };

            lblStatus = new Label
            {
                Location = new Point(20, 332),
                Size = new Size(464, 20),
                ForeColor = Color.LightGreen,
                Font = new Font("Segoe UI", 9F),
                Text = ""
            };

            btnUpdateNow = new Button
            {
                Text = "Update Now",
                Location = new Point(254, 348),
                Size = new Size(115, 34),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnUpdateNow.FlatAppearance.BorderSize = 0;
            btnUpdateNow.Click += BtnUpdateNow_Click;

            btnClose = new Button
            {
                Text = "Later",
                Location = new Point(379, 348),
                Size = new Size(105, 34),
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => { this.Close(); };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblVersionInfo);
            this.Controls.Add(lblFileSize);
            this.Controls.Add(lblNotesHeader);
            this.Controls.Add(rtbNotes);
            this.Controls.Add(progressBar);
            this.Controls.Add(lblStatus);
            this.Controls.Add(btnUpdateNow);
            this.Controls.Add(btnClose);
        }

        private void LoadData()
        {
            lblVersionInfo.Text = $"Current version: v{_updateInfo.CurrentVersion}  ➜  New version: v{_updateInfo.LatestVersion}";
            double sizeMb = (double)_updateInfo.FileSize / (1024 * 1024);
            lblFileSize.Text = $"Update size: {sizeMb:F2} MB ({_updateInfo.FileName})";
            rtbNotes.Text = string.IsNullOrWhiteSpace(_updateInfo.ReleaseNotes) ? "No additional release notes." : _updateInfo.ReleaseNotes;
        }

        private async void BtnUpdateNow_Click(object? sender, EventArgs e)
        {
            btnUpdateNow.Enabled = false;
            btnClose.Enabled = false;
            progressBar.Visible = true;
            lblStatus.Text = "Connecting to download server...";

            _cts = new CancellationTokenSource();
            string tempDir = Path.Combine(Path.GetTempPath(), "MSFC_Update");
            Directory.CreateDirectory(tempDir);
            string zipPath = Path.Combine(tempDir, string.IsNullOrEmpty(_updateInfo.FileName) ? "update.zip" : _updateInfo.FileName);

            try
            {
                var progress = new Progress<int>(percent =>
                {
                    progressBar.Value = Math.Min(100, Math.Max(0, percent));
                    lblStatus.Text = $"Downloading update: {percent}%";
                });

                await _updateService.DownloadFileWithProgressAsync(_updateInfo.DownloadUrl, zipPath, progress, _cts.Token);

                lblStatus.Text = "Verifying SHA256 checksum...";
                bool isValid = _updateService.VerifySha256(zipPath, _updateInfo.Sha256);
                if (!isValid)
                {
                    MessageBox.Show("SHA256 checksum does not match server file! Please try downloading again.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnUpdateNow.Enabled = true;
                    btnClose.Enabled = true;
                    return;
                }

                lblStatus.Text = "Download successful! Restarting application...";
                await System.Threading.Tasks.Task.Delay(1000);

                _updateService.ExecuteUpdatePackage(zipPath, _updateInfo.LatestVersion);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during update process: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnUpdateNow.Enabled = true;
                btnClose.Enabled = true;
                lblStatus.Text = "Update failed.";
            }
        }
    }
}

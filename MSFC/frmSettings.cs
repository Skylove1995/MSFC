using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Text.Json.Nodes;
using System.Windows.Forms;
using MySqlConnector;
using MSFC.Service;

namespace MSFC
{
    public partial class frmSettings : Form
    {
        private TabControl tabControl = null!;

        // Tab 1: Database
        private TextBox txtDbServer = null!;
        private TextBox txtDbPort = null!;
        private TextBox txtDbName = null!;
        private TextBox txtDbUser = null!;
        private TextBox txtDbPassword = null!;
        private Button btnTestDb = null!;

        // Tab 2: Scanner COM Ports
        private ComboBox cbPortScannerName = null!;
        private TextBox txtPortScannerBaud = null!;
        private ComboBox cbPortSfcName = null!;
        private TextBox txtPortSfcBaud = null!;

        // Tab 3: Update API
        private TextBox txtUpdateServerUrl = null!;
        private TextBox txtUpdateCheckApi = null!;
        private TextBox txtUpdateDownloadApi = null!;
        private TextBox txtCurrentVersion = null!;
        private Button btnCheckUpdate = null!;

        private Button btnSave = null!;
        private Button btnCancel = null!;

        public frmSettings()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "System Configuration - MSFC";
            this.Size = new Size(720, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 34);
            this.ForeColor = Color.White;

            tabControl = new TabControl
            {
                Location = new Point(12, 12),
                Size = new Size(680, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9.5F)
            };

            // Build Tabs
            TabPage tabDb = CreateDatabaseTab();
            TabPage tabCom = CreateComPortsTab();
            TabPage tabUpdate = CreateUpdateApiTab();

            tabControl.TabPages.Add(tabDb);
            tabControl.TabPages.Add(tabCom);
            tabControl.TabPages.Add(tabUpdate);

            btnSave = new Button
            {
                Text = "Save Config",
                Location = new Point(466, 475),
                Size = new Size(110, 34),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(582, 475),
                Size = new Size(110, 34),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.Close(); };

            this.Controls.Add(tabControl);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private TabPage CreateDatabaseTab()
        {
            TabPage tab = new TabPage("Database Connection")
            {
                BackColor = Color.FromArgb(38, 38, 42),
                ForeColor = Color.White
            };

            int top = 20;
            int inputWidth = 420;
            int labelX = 20;
            int inputX = 220;

            AddLabel(tab, "Server (IP):", labelX, top);
            txtDbServer = AddTextBox(tab, inputX, top, inputWidth);
            txtDbServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            top += 50;
            AddLabel(tab, "Port:", labelX, top);
            txtDbPort = AddTextBox(tab, inputX, top, inputWidth, "3306");
            txtDbPort.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            top += 50;
            AddLabel(tab, "Database Name:", labelX, top);
            txtDbName = AddTextBox(tab, inputX, top, inputWidth, "mex_mes");
            txtDbName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            top += 50;
            AddLabel(tab, "User:", labelX, top);
            txtDbUser = AddTextBox(tab, inputX, top, inputWidth, "root");
            txtDbUser.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            top += 50;
            AddLabel(tab, "Password:", labelX, top);
            txtDbPassword = AddTextBox(tab, inputX, top, inputWidth, "", true);
            txtDbPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            top += 55;
            btnTestDb = new Button
            {
                Text = "🔌 Test DB Connection",
                Location = new Point(inputX, top),
                Size = new Size(180, 32),
                BackColor = Color.FromArgb(45, 120, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnTestDb.FlatAppearance.BorderSize = 0;
            btnTestDb.Click += BtnTestDb_Click;
            tab.Controls.Add(btnTestDb);

            return tab;
        }

        private TabPage CreateComPortsTab()
        {
            TabPage tab = new TabPage("COM Ports")
            {
                BackColor = Color.FromArgb(38, 38, 42),
                ForeColor = Color.White
            };

            string[] availablePorts = SerialPort.GetPortNames();

            // Group 1: Scanner
            GroupBox gbScanner = new GroupBox
            {
                Text = "Input Device (Scanner)",
                Location = new Point(15, 15),
                Size = new Size(620, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            AddLabel(gbScanner, "Port / Mode:", 20, 35);
            cbPortScannerName = new ComboBox
            {
                Location = new Point(220, 32),
                Size = new Size(380, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            var scannerModes = new List<string> { "KeyboardHID" };
            scannerModes.AddRange(availablePorts.Length > 0 ? availablePorts : new[] { "COM1", "COM2", "COM3" });
            cbPortScannerName.Items.AddRange(scannerModes.ToArray());
            gbScanner.Controls.Add(cbPortScannerName);

            AddLabel(gbScanner, "Baud Rate:", 20, 75);
            txtPortScannerBaud = AddTextBox(gbScanner, 220, 72, 380, "9600");
            txtPortScannerBaud.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Group 2: SFC
            GroupBox gbSfc = new GroupBox
            {
                Text = "Port SFC (SFC System Connection)",
                Location = new Point(15, 150),
                Size = new Size(620, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            AddLabel(gbSfc, "COM Port Name:", 20, 35);
            cbPortSfcName = new ComboBox
            {
                Location = new Point(220, 32),
                Size = new Size(380, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            cbPortSfcName.Items.AddRange(availablePorts.Length > 0 ? availablePorts : new[] { "COM1", "COM2", "COM3" });
            gbSfc.Controls.Add(cbPortSfcName);

            AddLabel(gbSfc, "Baud Rate:", 20, 75);
            txtPortSfcBaud = AddTextBox(gbSfc, 220, 72, 380, "9600");
            txtPortSfcBaud.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            tab.Controls.Add(gbScanner);
            tab.Controls.Add(gbSfc);
            return tab;
        }

        private TabPage CreateUpdateApiTab()
        {
            TabPage tab = new TabPage("Auto Update (API)")
            {
                BackColor = Color.FromArgb(38, 38, 42),
                ForeColor = Color.White
            };

            int top = 20;
            int inputWidth = 420;
            int labelX = 20;
            int inputX = 220;

            AddLabel(tab, "Server API URL:", labelX, top);
            txtUpdateServerUrl = AddTextBox(tab, inputX, top, inputWidth, "http://10.195.89.20:9999");
            txtUpdateServerUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            top += 50;
            AddLabel(tab, "API Check Version:", labelX, top);
            txtUpdateCheckApi = AddTextBox(tab, inputX, top, inputWidth, "/api/app-update/check?currentVersion={0}");
            txtUpdateCheckApi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            top += 50;
            AddLabel(tab, "API Download ZIP:", labelX, top);
            txtUpdateDownloadApi = AddTextBox(tab, inputX, top, inputWidth, "/api/app-update/download/latest");
            txtUpdateDownloadApi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            top += 50;
            AddLabel(tab, "Current Version:", labelX, top);
            txtCurrentVersion = AddTextBox(tab, inputX, top, inputWidth, "1.0.0");
            txtCurrentVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            top += 55;
            btnCheckUpdate = new Button
            {
                Text = "🔍 Check For Updates Now",
                Location = new Point(inputX, top),
                Size = new Size(210, 34),
                BackColor = Color.FromArgb(0, 150, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnCheckUpdate.FlatAppearance.BorderSize = 0;
            btnCheckUpdate.Click += BtnCheckUpdate_Click;
            tab.Controls.Add(btnCheckUpdate);

            return tab;
        }

        private void LoadSettings()
        {
            // Load Database Connection String
            string connStr = AppConfigManager.GetValue("ConnectionStrings", "DefaultConnection", "");
            ParseConnectionString(connStr);

            // Load Serial Ports
            cbPortScannerName.Text = AppConfigManager.GetValue("PortScanner", "PortName", "COM2");
            txtPortScannerBaud.Text = AppConfigManager.GetValue("PortScanner", "BaudRate", "9600");

            cbPortSfcName.Text = AppConfigManager.GetValue("PortSFC", "PortName", "COM1");
            txtPortSfcBaud.Text = AppConfigManager.GetValue("PortSFC", "BaudRate", "9600");

            // Load Update API
            txtUpdateServerUrl.Text = AppConfigManager.GetValue("AppUpdate", "ServerUrl", "http://10.195.89.20:9999");
            txtUpdateCheckApi.Text = AppConfigManager.GetValue("AppUpdate", "CheckApi", "/api/app-update/check?currentVersion={0}");
            txtUpdateDownloadApi.Text = AppConfigManager.GetValue("AppUpdate", "DownloadApi", "/api/app-update/download/latest");
            txtCurrentVersion.Text = AppConfigManager.GetValue("AppUpdate", "CurrentVersion", "1.0.0");
        }

        private void ParseConnectionString(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr)) return;
            try
            {
                var builder = new MySqlConnectionStringBuilder(connStr);
                txtDbServer.Text = builder.Server;
                txtDbPort.Text = builder.Port.ToString();
                txtDbName.Text = builder.Database;
                txtDbUser.Text = builder.UserID;
                txtDbPassword.Text = builder.Password;
            }
            catch
            {
                // Fallback string parsing
                txtDbServer.Text = ExtractValue(connStr, "Server");
                txtDbPort.Text = ExtractValue(connStr, "Port", "3306");
                txtDbName.Text = ExtractValue(connStr, "Database");
                txtDbUser.Text = ExtractValue(connStr, "User");
                txtDbPassword.Text = ExtractValue(connStr, "Password");
            }
        }

        private string ExtractValue(string connStr, string key, string defaultValue = "")
        {
            foreach (var part in connStr.Split(';'))
            {
                var kv = part.Split('=');
                if (kv.Length == 2 && kv[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return kv[1].Trim();
                }
            }
            return defaultValue;
        }

        private string BuildConnectionString()
        {
            string server = txtDbServer.Text.Trim();
            string port = string.IsNullOrWhiteSpace(txtDbPort.Text) ? "3306" : txtDbPort.Text.Trim();
            string db = txtDbName.Text.Trim();
            string user = txtDbUser.Text.Trim();
            string pwd = txtDbPassword.Text;

            return $"Server={server};Port={port};Database={db};User={user};Password={pwd};";
        }

        private async void BtnTestDb_Click(object? sender, EventArgs e)
        {
            btnTestDb.Enabled = false;
            btnTestDb.Text = "⏳ Connecting...";

            string connStr = BuildConnectionString();
            try
            {
                using var conn = new MySqlConnection(connStr);
                await conn.OpenAsync();
                MessageBox.Show("✅ MySQL Database connection successful!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Connection failed: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTestDb.Enabled = true;
                btnTestDb.Text = "🔌 Test DB Connection";
            }
        }

        private async void BtnCheckUpdate_Click(object? sender, EventArgs e)
        {
            btnCheckUpdate.Enabled = false;
            btnCheckUpdate.Text = "⏳ Checking...";

            try
            {
                var updateService = new UpdateService();
                var response = await updateService.CheckForUpdatesAsync(txtUpdateServerUrl.Text.Trim(), txtUpdateCheckApi.Text.Trim(), txtCurrentVersion.Text.Trim());

                if (response != null && (response.HasUpdate || string.Compare(response.LatestVersion, response.CurrentVersion) > 0))
                {
                    var dlg = new frmUpdateNotice(response);
                    dlg.ShowDialog();
                }
                else if (response != null)
                {
                    MessageBox.Show($"Application is up to date ({response.CurrentVersion ?? txtCurrentVersion.Text})!", "Update Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Cannot connect to Update API server!", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update check error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckUpdate.Enabled = true;
                btnCheckUpdate.Text = "🔍 Check For Updates Now";
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // 1. Connection String
                string connStr = BuildConnectionString();
                AppConfigManager.UpdateSetting("ConnectionStrings", "DefaultConnection", connStr);

                // 2. Port Scanner
                AppConfigManager.UpdateSetting("PortScanner", "PortName", cbPortScannerName.Text.Trim());
                AppConfigManager.UpdateSetting("PortScanner", "BaudRate", txtPortScannerBaud.Text.Trim());

                // 3. Port SFC
                AppConfigManager.UpdateSetting("PortSFC", "PortName", cbPortSfcName.Text.Trim());
                AppConfigManager.UpdateSetting("PortSFC", "BaudRate", txtPortSfcBaud.Text.Trim());

                // 4. Update API
                AppConfigManager.UpdateSetting("AppUpdate", "ServerUrl", txtUpdateServerUrl.Text.Trim());
                AppConfigManager.UpdateSetting("AppUpdate", "CheckApi", txtUpdateCheckApi.Text.Trim());
                AppConfigManager.UpdateSetting("AppUpdate", "DownloadApi", txtUpdateDownloadApi.Text.Trim());
                AppConfigManager.UpdateSetting("AppUpdate", "CurrentVersion", txtCurrentVersion.Text.Trim());

                MessageBox.Show("✅ Configuration saved successfully to appsettings.json!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Label AddLabel(Control container, string text, int left, int top)
        {
            Label lbl = new Label
            {
                Text = text,
                Location = new Point(left, top + 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.Gainsboro
            };
            container.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(Control container, int left, int top, int width, string defaultText = "", bool isPassword = false)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(left, top),
                Size = new Size(width, 25),
                Text = defaultText,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.FromArgb(50, 50, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = isPassword
            };
            container.Controls.Add(txt);
            return txt;
        }
    }
}

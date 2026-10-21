using System;
using System.Drawing;
using System.Windows.Forms;
using MSFC.Service;

namespace MSFC
{
    public partial class frmAdminPassword : Form
    {
        public bool IsAuthenticated { get; private set; } = false;

        private TextBox txtPassword = null!;
        private Button btnLogin = null!;
        private Button btnCancel = null!;
        private Label lblMsg = null!;

        public frmAdminPassword()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Authentication";
            this.Size = new Size(390, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "Please enter Admin Password to access Settings:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.Gainsboro
            };

            txtPassword = new TextBox
            {
                Location = new Point(23, 50),
                Size = new Size(330, 27),
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };

            lblMsg = new Label
            {
                Location = new Point(23, 85),
                Size = new Size(330, 20),
                ForeColor = Color.LightCoral,
                Font = new Font("Segoe UI", 9F)
            };

            btnLogin = new Button
            {
                Text = "Confirm",
                Location = new Point(145, 115),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(253, 115),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblMsg);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnLogin;
            this.CancelButton = btnCancel;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            if (txtPassword.Text == "admin")
            {
                IsAuthenticated = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblMsg.Text = "Incorrect password!";
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
        }
    }
}

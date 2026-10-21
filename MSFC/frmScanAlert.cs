using System;
using System.Drawing;
using System.Windows.Forms;

namespace MSFC
{
    /// <summary>
    /// Blocking error popup for the scan queue.
    /// Only a mouse click or F1/Esc confirms it: the scanner's trailing Enter must never auto-close it.
    /// Characters typed by a keyboard-wedge scanner are forwarded so scans keep being queued.
    /// </summary>
    public class frmScanAlert : Form
    {
        private readonly Action<char>? _onScanChar;

        public frmScanAlert(string title, string pid, string detail, Action<char>? onScanChar)
        {
            _onScanChar = onScanChar;
            BuildUi(title, pid, detail);
        }

        private void BuildUi(string title, string pid, string detail)
        {
            Text = "MSFC - Alerta de escaneo / Scan alert";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = false;
            ShowInTaskbar = false;
            TopMost = true;
            KeyPreview = true;
            ClientSize = new Size(820, 460);
            BackColor = Color.FromArgb(32, 12, 14);
            Font = new Font("Segoe UI", 10F);

            var lblTitle = new Label
            {
                Text = $"🛑 {title}",
                Dock = DockStyle.Top,
                Height = 64,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(200, 32, 44),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold)
            };

            var lblPid = new Label
            {
                Text = $"PID: {pid}",
                Dock = DockStyle.Top,
                Height = 48,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(255, 214, 64),
                Font = new Font("Consolas", 18F, FontStyle.Bold)
            };

            var lblDetail = new Label
            {
                Text = detail,
                Dock = DockStyle.Fill,
                Padding = new Padding(28, 12, 28, 12),
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 13F),
                AutoEllipsis = true
            };

            var btnConfirm = new NoFocusButton
            {
                Text = "CONFIRMAR / CONFIRM   (F1)",
                Dock = DockStyle.Bottom,
                Height = 68,
                BackColor = Color.FromArgb(46, 160, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 190, 110);
            btnConfirm.Click += (_, _) => Confirm();

            var lblHint = new Label
            {
                Text = "Puede seguir escaneando: los PID quedan en cola.  •  You can keep scanning: PIDs stay queued.",
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(150, 150, 150),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };

            // Dock order: Fill must be added first so it takes the remaining space
            Controls.Add(lblDetail);
            Controls.Add(lblHint);
            Controls.Add(btnConfirm);
            Controls.Add(lblPid);
            Controls.Add(lblTitle);
        }

        private void Confirm()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData is Keys.F1 or Keys.Escape)
            {
                Confirm();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // Enter/Space/chars from the scanner never confirm: they go to the scan buffer
            _onScanChar?.Invoke(e.KeyChar);
            e.Handled = true;
        }

        /// <summary>Non-selectable so Enter/Space can't "click" it through keyboard focus.</summary>
        private sealed class NoFocusButton : Button
        {
            public NoFocusButton() => SetStyle(ControlStyles.Selectable, false);
        }
    }
}

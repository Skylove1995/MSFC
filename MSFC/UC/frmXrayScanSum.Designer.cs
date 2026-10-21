namespace MSFC.UC
{
    partial class frmXrayScanSum
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new System.Windows.Forms.Panel();
            lblTitle = new System.Windows.Forms.Label();
            lblLogTitle = new System.Windows.Forms.Label();
            lnkLogFilePath = new System.Windows.Forms.LinkLabel();
            lblInspectorTitle = new System.Windows.Forms.Label();
            txtInspectorInput = new System.Windows.Forms.TextBox();
            btnSaveInspector = new System.Windows.Forms.Button();
            btnOpenLog = new System.Windows.Forms.Button();
            pnlLeft = new System.Windows.Forms.Panel();
            lblInputTitle = new System.Windows.Forms.Label();
            txtPidInput = new System.Windows.Forms.TextBox();
            btnAddPid = new System.Windows.Forms.Button();
            lblStatus = new System.Windows.Forms.Label();
            pnlSlotsGroup = new System.Windows.Forms.GroupBox();
            lblSlot1 = new System.Windows.Forms.Label();
            lblSlot2 = new System.Windows.Forms.Label();
            lblSlot3 = new System.Windows.Forms.Label();
            lblSlot4 = new System.Windows.Forms.Label();
            lblSlot5 = new System.Windows.Forms.Label();
            lblSlot6 = new System.Windows.Forms.Label();
            pnlRight = new System.Windows.Forms.Panel();
            lblPreviewTitle = new System.Windows.Forms.Label();
            picPreview = new System.Windows.Forms.PictureBox();
            pnlBottomBtns = new System.Windows.Forms.Panel();
            btnPrint = new System.Windows.Forms.Button();
            btnClear = new System.Windows.Forms.Button();
            btnClose = new System.Windows.Forms.Button();
            pnlHeader.SuspendLayout();
            pnlLeft.SuspendLayout();
            pnlSlotsGroup.SuspendLayout();
            pnlRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            pnlBottomBtns.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = System.Drawing.Color.FromArgb(240, 244, 248);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblLogTitle);
            pnlHeader.Controls.Add(lnkLogFilePath);
            pnlHeader.Controls.Add(lblInspectorTitle);
            pnlHeader.Controls.Add(txtInspectorInput);
            pnlHeader.Controls.Add(btnSaveInspector);
            pnlHeader.Controls.Add(btnOpenLog);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Location = new System.Drawing.Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new System.Drawing.Size(960, 65);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblTitle.ForeColor = System.Drawing.Color.Navy;
            lblTitle.Location = new System.Drawing.Point(12, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(225, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "X-RAY SCAN (MAX 6 PIDs)";
            // 
            // lblLogTitle
            // 
            lblLogTitle.AutoSize = true;
            lblLogTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblLogTitle.Location = new System.Drawing.Point(245, 22);
            lblLogTitle.Name = "lblLogTitle";
            lblLogTitle.Size = new System.Drawing.Size(39, 20);
            lblLogTitle.TabIndex = 1;
            lblLogTitle.Text = "Log:";
            // 
            // lnkLogFilePath
            // 
            lnkLogFilePath.AutoSize = true;
            lnkLogFilePath.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lnkLogFilePath.Location = new System.Drawing.Point(285, 23);
            lnkLogFilePath.Name = "lnkLogFilePath";
            lnkLogFilePath.Size = new System.Drawing.Size(126, 20);
            lnkLogFilePath.TabIndex = 2;
            lnkLogFilePath.TabStop = true;
            lnkLogFilePath.Text = "XRay_PID_Log.txt";
            lnkLogFilePath.LinkClicked += lnkLogFilePath_LinkClicked;
            // 
            // lblInspectorTitle
            // 
            lblInspectorTitle.AutoSize = true;
            lblInspectorTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblInspectorTitle.Location = new System.Drawing.Point(425, 22);
            lblInspectorTitle.Name = "lblInspectorTitle";
            lblInspectorTitle.Size = new System.Drawing.Size(79, 20);
            lblInspectorTitle.TabIndex = 3;
            lblInspectorTitle.Text = "Inspector:";
            // 
            // txtInspectorInput
            // 
            txtInspectorInput.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            txtInspectorInput.Location = new System.Drawing.Point(505, 18);
            txtInspectorInput.Name = "txtInspectorInput";
            txtInspectorInput.Size = new System.Drawing.Size(100, 29);
            txtInspectorInput.TabIndex = 4;
            txtInspectorInput.TextChanged += txtInspectorInput_TextChanged;
            // 
            // btnSaveInspector
            // 
            btnSaveInspector.BackColor = System.Drawing.Color.DarkSlateGray;
            btnSaveInspector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSaveInspector.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnSaveInspector.ForeColor = System.Drawing.Color.White;
            btnSaveInspector.Location = new System.Drawing.Point(612, 17);
            btnSaveInspector.Name = "btnSaveInspector";
            btnSaveInspector.Size = new System.Drawing.Size(68, 32);
            btnSaveInspector.TabIndex = 5;
            btnSaveInspector.Text = "💾 Save";
            btnSaveInspector.UseVisualStyleBackColor = false;
            btnSaveInspector.Click += btnSaveInspector_Click;
            // 
            // btnOpenLog
            // 
            btnOpenLog.BackColor = System.Drawing.Color.SteelBlue;
            btnOpenLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnOpenLog.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnOpenLog.ForeColor = System.Drawing.Color.White;
            btnOpenLog.Location = new System.Drawing.Point(780, 16);
            btnOpenLog.Name = "btnOpenLog";
            btnOpenLog.Size = new System.Drawing.Size(160, 34);
            btnOpenLog.TabIndex = 6;
            btnOpenLog.Text = "📂 Open Log File";
            btnOpenLog.UseVisualStyleBackColor = false;
            btnOpenLog.Click += btnOpenLog_Click;
            // 
            // pnlLeft
            // 
            pnlLeft.Controls.Add(lblInputTitle);
            pnlLeft.Controls.Add(txtPidInput);
            pnlLeft.Controls.Add(btnAddPid);
            pnlLeft.Controls.Add(lblStatus);
            pnlLeft.Controls.Add(pnlSlotsGroup);
            pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            pnlLeft.Location = new System.Drawing.Point(0, 65);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new System.Drawing.Size(480, 596);
            pnlLeft.TabIndex = 1;
            // 
            // lblInputTitle
            // 
            lblInputTitle.AutoSize = true;
            lblInputTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblInputTitle.Location = new System.Drawing.Point(18, 15);
            lblInputTitle.Name = "lblInputTitle";
            lblInputTitle.Size = new System.Drawing.Size(165, 25);
            lblInputTitle.TabIndex = 0;
            lblInputTitle.Text = "Scan / Enter PID:";
            // 
            // txtPidInput
            // 
            txtPidInput.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            txtPidInput.Location = new System.Drawing.Point(20, 45);
            txtPidInput.Name = "txtPidInput";
            txtPidInput.Size = new System.Drawing.Size(330, 36);
            txtPidInput.TabIndex = 1;
            txtPidInput.KeyDown += txtPidInput_KeyDown;
            // 
            // btnAddPid
            // 
            btnAddPid.BackColor = System.Drawing.Color.SteelBlue;
            btnAddPid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAddPid.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnAddPid.ForeColor = System.Drawing.Color.White;
            btnAddPid.Location = new System.Drawing.Point(360, 44);
            btnAddPid.Name = "btnAddPid";
            btnAddPid.Size = new System.Drawing.Size(100, 38);
            btnAddPid.TabIndex = 2;
            btnAddPid.Text = "Add";
            btnAddPid.UseVisualStyleBackColor = false;
            btnAddPid.Click += btnAddPid_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblStatus.ForeColor = System.Drawing.Color.DarkSlateGray;
            lblStatus.Location = new System.Drawing.Point(18, 95);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(193, 25);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Progress: 0 / 6 PIDs";
            // 
            // pnlSlotsGroup
            // 
            pnlSlotsGroup.Controls.Add(lblSlot1);
            pnlSlotsGroup.Controls.Add(lblSlot2);
            pnlSlotsGroup.Controls.Add(lblSlot3);
            pnlSlotsGroup.Controls.Add(lblSlot4);
            pnlSlotsGroup.Controls.Add(lblSlot5);
            pnlSlotsGroup.Controls.Add(lblSlot6);
            pnlSlotsGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            pnlSlotsGroup.Location = new System.Drawing.Point(20, 130);
            pnlSlotsGroup.Name = "pnlSlotsGroup";
            pnlSlotsGroup.Size = new System.Drawing.Size(440, 440);
            pnlSlotsGroup.TabIndex = 4;
            pnlSlotsGroup.TabStop = false;
            pnlSlotsGroup.Text = "Scanned PID List";
            // 
            // lblSlot1
            // 
            lblSlot1.BackColor = System.Drawing.Color.WhiteSmoke;
            lblSlot1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblSlot1.Font = new System.Drawing.Font("Consolas", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSlot1.ForeColor = System.Drawing.Color.Gray;
            lblSlot1.Location = new System.Drawing.Point(15, 35);
            lblSlot1.Name = "lblSlot1";
            lblSlot1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            lblSlot1.Size = new System.Drawing.Size(410, 52);
            lblSlot1.TabIndex = 0;
            lblSlot1.Text = "1. [Not Scanned]";
            lblSlot1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSlot2
            // 
            lblSlot2.BackColor = System.Drawing.Color.WhiteSmoke;
            lblSlot2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblSlot2.Font = new System.Drawing.Font("Consolas", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSlot2.ForeColor = System.Drawing.Color.Gray;
            lblSlot2.Location = new System.Drawing.Point(15, 100);
            lblSlot2.Name = "lblSlot2";
            lblSlot2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            lblSlot2.Size = new System.Drawing.Size(410, 52);
            lblSlot2.TabIndex = 1;
            lblSlot2.Text = "2. [Not Scanned]";
            lblSlot2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSlot3
            // 
            lblSlot3.BackColor = System.Drawing.Color.WhiteSmoke;
            lblSlot3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblSlot3.Font = new System.Drawing.Font("Consolas", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSlot3.ForeColor = System.Drawing.Color.Gray;
            lblSlot3.Location = new System.Drawing.Point(15, 165);
            lblSlot3.Name = "lblSlot3";
            lblSlot3.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            lblSlot3.Size = new System.Drawing.Size(410, 52);
            lblSlot3.TabIndex = 2;
            lblSlot3.Text = "3. [Not Scanned]";
            lblSlot3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSlot4
            // 
            lblSlot4.BackColor = System.Drawing.Color.WhiteSmoke;
            lblSlot4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblSlot4.Font = new System.Drawing.Font("Consolas", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSlot4.ForeColor = System.Drawing.Color.Gray;
            lblSlot4.Location = new System.Drawing.Point(15, 230);
            lblSlot4.Name = "lblSlot4";
            lblSlot4.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            lblSlot4.Size = new System.Drawing.Size(410, 52);
            lblSlot4.TabIndex = 3;
            lblSlot4.Text = "4. [Not Scanned]";
            lblSlot4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSlot5
            // 
            lblSlot5.BackColor = System.Drawing.Color.WhiteSmoke;
            lblSlot5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblSlot5.Font = new System.Drawing.Font("Consolas", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSlot5.ForeColor = System.Drawing.Color.Gray;
            lblSlot5.Location = new System.Drawing.Point(15, 295);
            lblSlot5.Name = "lblSlot5";
            lblSlot5.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            lblSlot5.Size = new System.Drawing.Size(410, 52);
            lblSlot5.TabIndex = 4;
            lblSlot5.Text = "5. [Not Scanned]";
            lblSlot5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSlot6
            // 
            lblSlot6.BackColor = System.Drawing.Color.WhiteSmoke;
            lblSlot6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblSlot6.Font = new System.Drawing.Font("Consolas", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSlot6.ForeColor = System.Drawing.Color.Gray;
            lblSlot6.Location = new System.Drawing.Point(15, 360);
            lblSlot6.Name = "lblSlot6";
            lblSlot6.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            lblSlot6.Size = new System.Drawing.Size(410, 52);
            lblSlot6.TabIndex = 5;
            lblSlot6.Text = "6. [Not Scanned]";
            lblSlot6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(lblPreviewTitle);
            pnlRight.Controls.Add(picPreview);
            pnlRight.Controls.Add(pnlBottomBtns);
            pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlRight.Location = new System.Drawing.Point(480, 65);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new System.Drawing.Size(480, 596);
            pnlRight.TabIndex = 2;
            // 
            // lblPreviewTitle
            // 
            lblPreviewTitle.AutoSize = true;
            lblPreviewTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblPreviewTitle.Location = new System.Drawing.Point(15, 15);
            lblPreviewTitle.Name = "lblPreviewTitle";
            lblPreviewTitle.Size = new System.Drawing.Size(137, 25);
            lblPreviewTitle.TabIndex = 0;
            lblPreviewTitle.Text = "Label Preview:";
            // 
            // picPreview
            // 
            picPreview.BackColor = System.Drawing.Color.White;
            picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            picPreview.Location = new System.Drawing.Point(45, 45);
            picPreview.Name = "picPreview";
            picPreview.Size = new System.Drawing.Size(360, 460);
            picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 1;
            picPreview.TabStop = false;
            picPreview.Paint += picPreview_Paint;
            // 
            // pnlBottomBtns
            // 
            pnlBottomBtns.Controls.Add(btnPrint);
            pnlBottomBtns.Controls.Add(btnClear);
            pnlBottomBtns.Controls.Add(btnClose);
            pnlBottomBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlBottomBtns.Location = new System.Drawing.Point(0, 516);
            pnlBottomBtns.Name = "pnlBottomBtns";
            pnlBottomBtns.Size = new System.Drawing.Size(480, 80);
            pnlBottomBtns.TabIndex = 2;
            // 
            // btnPrint
            // 
            btnPrint.BackColor = System.Drawing.Color.MediumSeaGreen;
            btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnPrint.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnPrint.ForeColor = System.Drawing.Color.White;
            btnPrint.Location = new System.Drawing.Point(15, 15);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new System.Drawing.Size(140, 50);
            btnPrint.TabIndex = 0;
            btnPrint.Text = "🖨️ Print Label";
            btnPrint.UseVisualStyleBackColor = false;
            btnPrint.Click += btnPrint_Click;
            // 
            // btnClear
            // 
            btnClear.BackColor = System.Drawing.Color.DarkOrange;
            btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClear.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnClear.ForeColor = System.Drawing.Color.White;
            btnClear.Location = new System.Drawing.Point(170, 15);
            btnClear.Name = "btnClear";
            btnClear.Size = new System.Drawing.Size(140, 50);
            btnClear.TabIndex = 1;
            btnClear.Text = "🔄 Reset";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += btnClear_Click;
            // 
            // btnClose
            // 
            btnClose.BackColor = System.Drawing.Color.DimGray;
            btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClose.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnClose.ForeColor = System.Drawing.Color.White;
            btnClose.Location = new System.Drawing.Point(325, 15);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(140, 50);
            btnClose.TabIndex = 2;
            btnClose.Text = "❌ Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // frmXrayScanSum
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(960, 661);
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            Controls.Add(pnlHeader);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmXrayScanSum";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "X-Ray Inspection & PID Scan";
            Load += frmXrayScanSum_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlLeft.ResumeLayout(false);
            pnlLeft.PerformLayout();
            pnlSlotsGroup.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            pnlBottomBtns.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblLogTitle;
        private System.Windows.Forms.LinkLabel lnkLogFilePath;
        private System.Windows.Forms.Label lblInspectorTitle;
        private System.Windows.Forms.TextBox txtInspectorInput;
        private System.Windows.Forms.Button btnSaveInspector;
        private System.Windows.Forms.Button btnOpenLog;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Label lblInputTitle;
        private System.Windows.Forms.TextBox txtPidInput;
        private System.Windows.Forms.Button btnAddPid;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox pnlSlotsGroup;
        private System.Windows.Forms.Label lblSlot1;
        private System.Windows.Forms.Label lblSlot2;
        private System.Windows.Forms.Label lblSlot3;
        private System.Windows.Forms.Label lblSlot4;
        private System.Windows.Forms.Label lblSlot5;
        private System.Windows.Forms.Label lblSlot6;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblPreviewTitle;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Panel pnlBottomBtns;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnClose;
    }
}

namespace MSFC.UC
{
    partial class Summary
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblSubTitle = new Label();
            lblTitle = new Label();
            pnlFilter = new Panel();
            btnLoad = new Button();
            dtToTime = new DateTimePicker();
            lblTo = new Label();
            dtFromTime = new DateTimePicker();
            lblFrom = new Label();
            pnlKpi = new Panel();
            tblKpi = new TableLayoutPanel();
            pnlKpi1 = new Panel();
            lblKpiTotalScanVal = new Label();
            lblKpiTotalScanTitle = new Label();
            pnlKpi2 = new Panel();
            lblKpiTotalQtyVal = new Label();
            lblKpiTotalQtyTitle = new Label();
            pnlKpi3 = new Panel();
            lblKpiPartCountVal = new Label();
            lblKpiPartCountTitle = new Label();
            pnlData = new Panel();
            dvSummary = new DataGridView();
            pnlHeader.SuspendLayout();
            pnlFilter.SuspendLayout();
            pnlKpi.SuspendLayout();
            tblKpi.SuspendLayout();
            pnlKpi1.SuspendLayout();
            pnlKpi2.SuspendLayout();
            pnlKpi3.SuspendLayout();
            pnlData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dvSummary).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(15, 23, 42);
            pnlHeader.Controls.Add(lblSubTitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1350, 65);
            pnlHeader.TabIndex = 0;
            // 
            // lblSubTitle
            // 
            lblSubTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblSubTitle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSubTitle.ForeColor = Color.FromArgb(148, 163, 184);
            lblSubTitle.Location = new Point(900, 20);
            lblSubTitle.Name = "lblSubTitle";
            lblSubTitle.Size = new Size(430, 25);
            lblSubTitle.TabIndex = 1;
            lblSubTitle.Text = "Client IP / IP Cliente: 127.0.0.1";
            lblSubTitle.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(680, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "📊 SCAN SUMMARY REPORT / REPORTE DE PRODUCCIÓN";
            // 
            // pnlFilter
            // 
            pnlFilter.BackColor = Color.FromArgb(30, 41, 59);
            pnlFilter.Controls.Add(btnLoad);
            pnlFilter.Controls.Add(dtToTime);
            pnlFilter.Controls.Add(lblTo);
            pnlFilter.Controls.Add(dtFromTime);
            pnlFilter.Controls.Add(lblFrom);
            pnlFilter.Dock = DockStyle.Top;
            pnlFilter.Location = new Point(0, 65);
            pnlFilter.Name = "pnlFilter";
            pnlFilter.Size = new Size(1350, 55);
            pnlFilter.TabIndex = 1;
            // 
            // btnLoad
            // 
            btnLoad.BackColor = Color.FromArgb(37, 99, 235);
            btnLoad.Cursor = Cursors.Hand;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLoad.ForeColor = Color.White;
            btnLoad.Location = new Point(650, 10);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(170, 35);
            btnLoad.TabIndex = 4;
            btnLoad.Text = "🔄 Load / Cargar";
            btnLoad.UseVisualStyleBackColor = false;
            btnLoad.Click += btnLoad_Click;
            // 
            // dtToTime
            // 
            dtToTime.CustomFormat = "dd/MM/yyyy";
            dtToTime.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtToTime.Format = DateTimePickerFormat.Custom;
            dtToTime.Location = new Point(475, 10);
            dtToTime.Name = "dtToTime";
            dtToTime.Size = new Size(150, 34);
            dtToTime.TabIndex = 3;
            // 
            // lblTo
            // 
            lblTo.AutoSize = true;
            lblTo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTo.ForeColor = Color.White;
            lblTo.Location = new Point(340, 13);
            lblTo.Name = "lblTo";
            lblTo.Size = new Size(125, 25);
            lblTo.TabIndex = 2;
            lblTo.Text = "To / Hasta:";
            // 
            // dtFromTime
            // 
            dtFromTime.CustomFormat = "dd/MM/yyyy";
            dtFromTime.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtFromTime.Format = DateTimePickerFormat.Custom;
            dtFromTime.Location = new Point(160, 10);
            dtFromTime.Name = "dtFromTime";
            dtFromTime.Size = new Size(150, 34);
            dtFromTime.TabIndex = 1;
            // 
            // lblFrom
            // 
            lblFrom.AutoSize = true;
            lblFrom.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFrom.ForeColor = Color.White;
            lblFrom.Location = new Point(20, 13);
            lblFrom.Name = "lblFrom";
            lblFrom.Size = new Size(130, 25);
            lblFrom.TabIndex = 0;
            lblFrom.Text = "From / Desde:";
            // 
            // pnlKpi
            // 
            pnlKpi.BackColor = Color.FromArgb(241, 245, 249);
            pnlKpi.Controls.Add(tblKpi);
            pnlKpi.Dock = DockStyle.Top;
            pnlKpi.Location = new Point(0, 120);
            pnlKpi.Name = "pnlKpi";
            pnlKpi.Padding = new Padding(15, 10, 15, 10);
            pnlKpi.Size = new Size(1350, 85);
            pnlKpi.TabIndex = 2;
            // 
            // tblKpi
            // 
            tblKpi.ColumnCount = 3;
            tblKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tblKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tblKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tblKpi.Controls.Add(pnlKpi1, 0, 0);
            tblKpi.Controls.Add(pnlKpi2, 1, 0);
            tblKpi.Controls.Add(pnlKpi3, 2, 0);
            tblKpi.Dock = DockStyle.Fill;
            tblKpi.Location = new Point(15, 10);
            tblKpi.Name = "tblKpi";
            tblKpi.RowCount = 1;
            tblKpi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblKpi.Size = new Size(1320, 65);
            tblKpi.TabIndex = 0;
            // 
            // pnlKpi1
            // 
            pnlKpi1.BackColor = Color.White;
            pnlKpi1.BorderStyle = BorderStyle.FixedSingle;
            pnlKpi1.Controls.Add(lblKpiTotalScanVal);
            pnlKpi1.Controls.Add(lblKpiTotalScanTitle);
            pnlKpi1.Dock = DockStyle.Fill;
            pnlKpi1.Location = new Point(3, 3);
            pnlKpi1.Name = "pnlKpi1";
            pnlKpi1.Size = new Size(434, 59);
            pnlKpi1.TabIndex = 0;
            // 
            // lblKpiTotalScanVal
            // 
            lblKpiTotalScanVal.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblKpiTotalScanVal.ForeColor = Color.FromArgb(14, 165, 233);
            lblKpiTotalScanVal.Location = new Point(230, 10);
            lblKpiTotalScanVal.Name = "lblKpiTotalScanVal";
            lblKpiTotalScanVal.Size = new Size(190, 38);
            lblKpiTotalScanVal.TabIndex = 1;
            lblKpiTotalScanVal.Text = "0";
            lblKpiTotalScanVal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblKpiTotalScanTitle
            // 
            lblKpiTotalScanTitle.AutoSize = true;
            lblKpiTotalScanTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblKpiTotalScanTitle.ForeColor = Color.FromArgb(71, 85, 105);
            lblKpiTotalScanTitle.Location = new Point(8, 16);
            lblKpiTotalScanTitle.Name = "lblKpiTotalScanTitle";
            lblKpiTotalScanTitle.Size = new Size(220, 25);
            lblKpiTotalScanTitle.TabIndex = 0;
            lblKpiTotalScanTitle.Text = "📌 Total Scans / Escaneos";
            // 
            // pnlKpi2
            // 
            pnlKpi2.BackColor = Color.White;
            pnlKpi2.BorderStyle = BorderStyle.FixedSingle;
            pnlKpi2.Controls.Add(lblKpiTotalQtyVal);
            pnlKpi2.Controls.Add(lblKpiTotalQtyTitle);
            pnlKpi2.Dock = DockStyle.Fill;
            pnlKpi2.Location = new Point(443, 3);
            pnlKpi2.Name = "pnlKpi2";
            pnlKpi2.Size = new Size(434, 59);
            pnlKpi2.TabIndex = 1;
            // 
            // lblKpiTotalQtyVal
            // 
            lblKpiTotalQtyVal.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblKpiTotalQtyVal.ForeColor = Color.FromArgb(16, 185, 129);
            lblKpiTotalQtyVal.Location = new Point(230, 10);
            lblKpiTotalQtyVal.Name = "lblKpiTotalQtyVal";
            lblKpiTotalQtyVal.Size = new Size(190, 38);
            lblKpiTotalQtyVal.TabIndex = 1;
            lblKpiTotalQtyVal.Text = "0";
            lblKpiTotalQtyVal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblKpiTotalQtyTitle
            // 
            lblKpiTotalQtyTitle.AutoSize = true;
            lblKpiTotalQtyTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblKpiTotalQtyTitle.ForeColor = Color.FromArgb(71, 85, 105);
            lblKpiTotalQtyTitle.Location = new Point(8, 16);
            lblKpiTotalQtyTitle.Name = "lblKpiTotalQtyTitle";
            lblKpiTotalQtyTitle.Size = new Size(220, 25);
            lblKpiTotalQtyTitle.TabIndex = 0;
            lblKpiTotalQtyTitle.Text = "📦 Total Qty / Cantidad";
            // 
            // pnlKpi3
            // 
            pnlKpi3.BackColor = Color.White;
            pnlKpi3.BorderStyle = BorderStyle.FixedSingle;
            pnlKpi3.Controls.Add(lblKpiPartCountVal);
            pnlKpi3.Controls.Add(lblKpiPartCountTitle);
            pnlKpi3.Dock = DockStyle.Fill;
            pnlKpi3.Location = new Point(883, 3);
            pnlKpi3.Name = "pnlKpi3";
            pnlKpi3.Size = new Size(434, 59);
            pnlKpi3.TabIndex = 2;
            // 
            // lblKpiPartCountVal
            // 
            lblKpiPartCountVal.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblKpiPartCountVal.ForeColor = Color.FromArgb(139, 92, 246);
            lblKpiPartCountVal.Location = new Point(230, 10);
            lblKpiPartCountVal.Name = "lblKpiPartCountVal";
            lblKpiPartCountVal.Size = new Size(190, 38);
            lblKpiPartCountVal.TabIndex = 1;
            lblKpiPartCountVal.Text = "0";
            lblKpiPartCountVal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblKpiPartCountTitle
            // 
            lblKpiPartCountTitle.AutoSize = true;
            lblKpiPartCountTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblKpiPartCountTitle.ForeColor = Color.FromArgb(71, 85, 105);
            lblKpiPartCountTitle.Location = new Point(8, 16);
            lblKpiPartCountTitle.Name = "lblKpiPartCountTitle";
            lblKpiPartCountTitle.Size = new Size(220, 25);
            lblKpiPartCountTitle.TabIndex = 0;
            lblKpiPartCountTitle.Text = "🏷️ Part Numbers / Partes";
            // 
            // pnlData
            // 
            pnlData.Controls.Add(dvSummary);
            pnlData.Dock = DockStyle.Fill;
            pnlData.Location = new Point(0, 205);
            pnlData.Name = "pnlData";
            pnlData.Padding = new Padding(15, 5, 15, 15);
            pnlData.Size = new Size(1350, 495);
            pnlData.TabIndex = 3;
            // 
            // dvSummary
            // 
            dvSummary.AllowUserToAddRows = false;
            dvSummary.AllowUserToDeleteRows = false;
            dvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dvSummary.BackgroundColor = Color.White;
            dvSummary.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle1.SelectionForeColor = Color.White;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dvSummary.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dvSummary.ColumnHeadersHeight = 42;
            dvSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(224, 231, 255);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dvSummary.DefaultCellStyle = dataGridViewCellStyle2;
            dvSummary.Dock = DockStyle.Fill;
            dvSummary.EnableHeadersVisualStyles = false;
            dvSummary.GridColor = Color.FromArgb(226, 232, 240);
            dvSummary.Location = new Point(15, 5);
            dvSummary.Name = "dvSummary";
            dvSummary.ReadOnly = true;
            dvSummary.RowHeadersWidth = 40;
            dvSummary.RowTemplate.Height = 36;
            dvSummary.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dvSummary.Size = new Size(1320, 475);
            dvSummary.TabIndex = 0;
            // 
            // Summary
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1350, 700);
            Controls.Add(pnlData);
            Controls.Add(pnlKpi);
            Controls.Add(pnlFilter);
            Controls.Add(pnlHeader);
            MinimumSize = new Size(1200, 600);
            Name = "Summary";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Scan Summary Report / Reporte de Resumen de Escaneo";
            Load += Summary_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFilter.ResumeLayout(false);
            pnlFilter.PerformLayout();
            pnlKpi.ResumeLayout(false);
            tblKpi.ResumeLayout(false);
            pnlKpi1.ResumeLayout(false);
            pnlKpi1.PerformLayout();
            pnlKpi2.ResumeLayout(false);
            pnlKpi2.PerformLayout();
            pnlKpi3.ResumeLayout(false);
            pnlKpi3.PerformLayout();
            pnlData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dvSummary).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubTitle;
        private Panel pnlFilter;
        private Label lblFrom;
        private DateTimePicker dtFromTime;
        private Label lblTo;
        private DateTimePicker dtToTime;
        private Button btnLoad;
        private Panel pnlKpi;
        private TableLayoutPanel tblKpi;
        private Panel pnlKpi1;
        private Label lblKpiTotalScanTitle;
        private Label lblKpiTotalScanVal;
        private Panel pnlKpi2;
        private Label lblKpiTotalQtyTitle;
        private Label lblKpiTotalQtyVal;
        private Panel pnlKpi3;
        private Label lblKpiPartCountTitle;
        private Label lblKpiPartCountVal;
        private Panel pnlData;
        private DataGridView dvSummary;
    }
}

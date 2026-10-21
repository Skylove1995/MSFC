namespace MSFC.UC
{
    partial class RePrint
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            flowLayoutPanel1 = new FlowLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            tbReInspector2 = new TextBox();
            tbReInspector1 = new TextBox();
            tbRePartNo = new TextBox();
            modelName = new Label();
            Date = new Label();
            partNo = new Label();
            qTy = new Label();
            inspector1 = new Label();
            inspector2 = new Label();
            tbReModelName = new TextBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            btnConfirmRePrint = new Button();
            btnCancelRePrint = new Button();
            dtReDate = new DateTimePicker();
            nbReQty = new NumericUpDown();
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nbReQty).BeginInit();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(tableLayoutPanel1);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(593, 540);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = SystemColors.ActiveCaption;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.57143F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71.42857F));
            tableLayoutPanel1.Controls.Add(tbReInspector2, 1, 5);
            tableLayoutPanel1.Controls.Add(tbReInspector1, 1, 4);
            tableLayoutPanel1.Controls.Add(tbRePartNo, 1, 2);
            tableLayoutPanel1.Controls.Add(modelName, 0, 0);
            tableLayoutPanel1.Controls.Add(Date, 0, 1);
            tableLayoutPanel1.Controls.Add(partNo, 0, 2);
            tableLayoutPanel1.Controls.Add(qTy, 0, 3);
            tableLayoutPanel1.Controls.Add(inspector1, 0, 4);
            tableLayoutPanel1.Controls.Add(inspector2, 0, 5);
            tableLayoutPanel1.Controls.Add(tbReModelName, 1, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 6);
            tableLayoutPanel1.Controls.Add(dtReDate, 1, 1);
            tableLayoutPanel1.Controls.Add(nbReQty, 1, 3);
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.36782F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.3678169F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.3678169F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.3678169F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.3678169F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.3678169F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 13.7931042F));
            tableLayoutPanel1.Size = new Size(590, 537);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tbReInspector2
            // 
            tbReInspector2.Anchor = AnchorStyles.None;
            tbReInspector2.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbReInspector2.Location = new Point(193, 405);
            tbReInspector2.Name = "tbReInspector2";
            tbReInspector2.Size = new Size(371, 37);
            tbReInspector2.TabIndex = 11;
            // 
            // tbReInspector1
            // 
            tbReInspector1.Anchor = AnchorStyles.None;
            tbReInspector1.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbReInspector1.Location = new Point(193, 328);
            tbReInspector1.Name = "tbReInspector1";
            tbReInspector1.Size = new Size(371, 37);
            tbReInspector1.TabIndex = 10;
            // 
            // tbRePartNo
            // 
            tbRePartNo.Anchor = AnchorStyles.None;
            tbRePartNo.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbRePartNo.Location = new Point(193, 174);
            tbRePartNo.Name = "tbRePartNo";
            tbRePartNo.Size = new Size(371, 37);
            tbRePartNo.TabIndex = 8;
            // 
            // modelName
            // 
            modelName.Anchor = AnchorStyles.None;
            modelName.AutoSize = true;
            modelName.Location = new Point(22, 26);
            modelName.Name = "modelName";
            modelName.Size = new Size(124, 25);
            modelName.TabIndex = 0;
            modelName.Text = "Model Name :";
            // 
            // Date
            // 
            Date.Anchor = AnchorStyles.None;
            Date.AutoSize = true;
            Date.Location = new Point(55, 103);
            Date.Name = "Date";
            Date.Size = new Size(58, 25);
            Date.TabIndex = 1;
            Date.Text = "Date :";
            // 
            // partNo
            // 
            partNo.Anchor = AnchorStyles.None;
            partNo.AutoSize = true;
            partNo.Location = new Point(46, 180);
            partNo.Name = "partNo";
            partNo.Size = new Size(75, 25);
            partNo.TabIndex = 2;
            partNo.Text = "Part No:";
            // 
            // qTy
            // 
            qTy.Anchor = AnchorStyles.None;
            qTy.AutoSize = true;
            qTy.Location = new Point(57, 257);
            qTy.Name = "qTy";
            qTy.Size = new Size(54, 25);
            qTy.TabIndex = 3;
            qTy.Text = "Q'ty :";
            // 
            // inspector1
            // 
            inspector1.Anchor = AnchorStyles.None;
            inspector1.AutoSize = true;
            inspector1.Location = new Point(31, 334);
            inspector1.Name = "inspector1";
            inspector1.Size = new Size(105, 25);
            inspector1.TabIndex = 4;
            inspector1.Text = "Inspector 1:";
            // 
            // inspector2
            // 
            inspector2.Anchor = AnchorStyles.None;
            inspector2.AutoSize = true;
            inspector2.Location = new Point(31, 411);
            inspector2.Name = "inspector2";
            inspector2.Size = new Size(105, 25);
            inspector2.TabIndex = 5;
            inspector2.Text = "Inspector 2:";
            // 
            // tbReModelName
            // 
            tbReModelName.Anchor = AnchorStyles.None;
            tbReModelName.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbReModelName.Location = new Point(193, 20);
            tbReModelName.Name = "tbReModelName";
            tbReModelName.Size = new Size(371, 37);
            tbReModelName.TabIndex = 6;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(btnCancelRePrint, 1, 0);
            tableLayoutPanel2.Controls.Add(btnConfirmRePrint, 0, 0);
            tableLayoutPanel2.Location = new Point(171, 465);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(416, 69);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // btnConfirmRePrint
            // 
            btnConfirmRePrint.Anchor = AnchorStyles.None;
            btnConfirmRePrint.BackColor = Color.MediumSpringGreen;
            btnConfirmRePrint.Location = new Point(46, 9);
            btnConfirmRePrint.Name = "btnConfirmRePrint";
            btnConfirmRePrint.Size = new Size(115, 50);
            btnConfirmRePrint.TabIndex = 0;
            btnConfirmRePrint.Text = "Confirm";
            btnConfirmRePrint.UseVisualStyleBackColor = false;
            // 
            // btnCancelRePrint
            // 
            btnCancelRePrint.Anchor = AnchorStyles.None;
            btnCancelRePrint.BackColor = SystemColors.AppWorkspace;
            btnCancelRePrint.ForeColor = SystemColors.ActiveCaptionText;
            btnCancelRePrint.Location = new Point(254, 9);
            btnCancelRePrint.Name = "btnCancelRePrint";
            btnCancelRePrint.Size = new Size(115, 50);
            btnCancelRePrint.TabIndex = 1;
            btnCancelRePrint.Text = "Cancel";
            btnCancelRePrint.UseVisualStyleBackColor = false;
            // 
            // dtReDate
            // 
            dtReDate.Anchor = AnchorStyles.None;
            dtReDate.CalendarFont = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtReDate.Format = DateTimePickerFormat.Short;
            dtReDate.Location = new Point(194, 95);
            dtReDate.Name = "dtReDate";
            dtReDate.Size = new Size(369, 31);
            dtReDate.TabIndex = 13;
            // 
            // nbReQty
            // 
            nbReQty.Anchor = AnchorStyles.None;
            nbReQty.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            nbReQty.Location = new Point(319, 251);
            nbReQty.Name = "nbReQty";
            nbReQty.Size = new Size(120, 37);
            nbReQty.TabIndex = 14;
            // 
            // RePrint
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(593, 540);
            Controls.Add(flowLayoutPanel1);
            MaximizeBox = false;
            Name = "RePrint";
            flowLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nbReQty).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel1;
        private Label modelName;
        private Label Date;
        private Label partNo;
        private Label qTy;
        private Label inspector1;
        private Label inspector2;
        private TextBox tbReModelName;
        private TextBox tbReInspector2;
        private TextBox tbReInspector1;
        private TextBox tbRePartNo;
        private TableLayoutPanel tableLayoutPanel2;
        private Button btnCancelRePrint;
        private Button btnConfirmRePrint;
        private DateTimePicker dtReDate;
        private NumericUpDown nbReQty;
    }
}

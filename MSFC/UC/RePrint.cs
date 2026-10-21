using MSFC.Dto;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSFC.UC
{
    public partial class RePrint : Form
    {
        // Khai báo biến cho việc in ấn (giống Form1)
        private ManufacturingTagDto _tagLabel = new ManufacturingTagDto();

        // Cấu hình in ấn (Copy từ Form1 để đảm bảo tem in ra y hệt)
        float _offsetXmm = 0f;
        float _offsetYmm = -2.0f;
        float _labelWidthMm = 55.0f;
        float _labelHeightMm = 45.0f;
        float _topMarginMm = 2.0f;

        public RePrint()
        {
            InitializeComponent();
            try { this.Icon = Form1.GetAppIcon(); } catch { }

            // Gán sự kiện cho các nút bấm
            this.btnConfirmRePrint.Click += new System.EventHandler(this.btnConfirmRePrint_Click);
            this.btnCancelRePrint.Click += new System.EventHandler(this.btnCancelRePrint_Click);

            // Mặc định ngày hiện tại
            dtReDate.Value = DateTime.Now;
        }

        // 1. Xử lý nút CANCEL
        private void btnCancelRePrint_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 2. Xử lý nút CONFIRM
        private void btnConfirmRePrint_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu nhập
            if (string.IsNullOrWhiteSpace(tbReModelName.Text) ||
                string.IsNullOrWhiteSpace(tbRePartNo.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Model Name và Part No.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Yêu cầu mật khẩu
            string password = PromptForPassword();

            // Nếu người dùng ấn Cancel hoặc đóng hộp thoại pass -> password sẽ là null
            if (password == null) return;

            if (password == "haengsungmx")
            {
                PerformPrint();
            }
            else
            {
                MessageBox.Show("Mật khẩu không đúng!", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm hiển thị hộp thoại nhập mật khẩu
        private string PromptForPassword()
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Xác thực quản trị viên",
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label() { Left = 30, Top = 20, Text = "Nhập mật khẩu xác thực (Password):", Width = 300 };
            TextBox textBox = new TextBox() { Left = 30, Top = 50, Width = 320, PasswordChar = '*' };
            Button confirmation = new Button() { Text = "Ok", Left = 230, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancel", Left = 120, Width = 100, Top = 90, DialogResult = DialogResult.Cancel };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }

        // 3. Logic chuẩn bị dữ liệu và gọi lệnh in
        private void PerformPrint()
        {
            try
            {
                // Tạo data từ thông tin người dùng nhập
                _tagLabel = new ManufacturingTagDto
                {
                    ModelName  = tbReModelName.Text.Trim(),
                    Date       = dtReDate.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                    PartNo     = tbRePartNo.Text.Trim(),
                    Quantity   = (int)nbReQty.Value,
                    Inspector1 = tbReInspector1.Text.Trim(),
                    Inspector2 = tbReInspector2.Text.Trim(),
                    TagId      = 0,
                    PidList    = new List<string>()
                };

                // Gọi hàm in
                PrintTag(_tagLabel);

                MessageBox.Show("Đã gửi lệnh in thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region COPY OF PRINTING LOGIC FROM FORM1.CS

        static int MmToHundredths(float mm) => (int)Math.Round(mm / 25.4f * 100f);

        void PrintTag(ManufacturingTagDto tag)
        {
            try
            {
                var doc = new PrintDocument();

                if (!doc.PrinterSettings.IsValid) throw new Exception("Không tìm thấy máy in hợp lệ.");

                doc.PrintController = new System.Drawing.Printing.StandardPrintController();
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

                doc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                doc.OriginAtMargins = true;
                doc.DefaultPageSettings.Landscape = false;

                _tagLabel = tag;
                doc.PrintPage += GenerateLabel_FitMarginBounds;

                doc.Print();
            }
            catch (Exception)
            {
                throw; // Ném lỗi ra để hàm gọi xử lý hiển thị MessageBox
            }
        }

        void GenerateLabel_FitMarginBounds(object sender, PrintPageEventArgs e)
        {
            if (_tagLabel == null) { e.Cancel = true; return; }
            var g = e.Graphics;

            try
            {
                Rectangle deviceBounds = e.MarginBounds;

                // Thiết lập kích thước thiết kế theo biến cấu hình
                float designW = MmToHundredths(_labelWidthMm);
                float designH = MmToHundredths(_labelHeightMm);

                // Tính scale
                float scale = (float)deviceBounds.Width / designW;
                if (scale > 1.0f) scale = 1.0f;

                float drawnW = designW * scale;

                // Tính toạ độ
                float translateX   = deviceBounds.Left + (deviceBounds.Width - drawnW) / 2f;
                float topMarginUnit = MmToHundredths(_topMarginMm);
                float offsetUnit    = MmToHundredths(_offsetYmm);
                float translateY   = deviceBounds.Top + topMarginUnit + offsetUnit;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("[ERROR] GenerateLabel: " + ex.Message);
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

        static void DrawFitText(Graphics g, string text, Font baseFont, Brush brush, RectangleF rect, float minPointSize = 6.0f)
        {
            var fmt = new StringFormat
            {
                Alignment     = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
                FormatFlags   = StringFormatFlags.NoWrap
            };

            for (float sz = baseFont.Size; sz >= minPointSize; sz -= 0.5f)
            {
                using var f        = new Font(baseFont.FontFamily, sz, baseFont.Style, GraphicsUnit.Point);
                var       measured = g.MeasureString(text, f, (int)Math.Ceiling(rect.Width), fmt);

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

        #endregion
    }
}
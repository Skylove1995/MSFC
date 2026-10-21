using Microsoft.EntityFrameworkCore;
using MSFC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSFC.UC
{
    public partial class Summary : Form
    {
        private readonly IDbContextFactory<MMesDbContext> _dbFactory;
        private readonly string _clientIp;

        public Summary(IDbContextFactory<MMesDbContext> dbFactory, string clientIp)
        {
            _dbFactory = dbFactory;
            _clientIp = clientIp;

            InitializeComponent();
            try { this.Icon = Form1.GetAppIcon(); } catch { }

            EnableDoubleBuffering(dvSummary);

            lblSubTitle.Text = $"🖥️ Client IP / IP Cliente: {_clientIp}";
            dtFromTime.Value = DateTime.Today;
            dtToTime.Value = DateTime.Today;

            // Đăng ký sự kiện định dạng Cell cho DataGridView
            dvSummary.CellFormatting += DvSummary_CellFormatting;
        }

        private static void EnableDoubleBuffering(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, control, new object[] { true });
        }

        private void Summary_Load(object sender, EventArgs e)
        {
            LoadSummary();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadSummary();
        }

        private void LoadSummary()
        {
            if (dtFromTime.Value.Date > dtToTime.Value.Date)
            {
                MessageBox.Show(
                    "Invalid date range: 'From Date' cannot be greater than 'To Date'.\n" +
                    "Rango de fechas inválido: 'Desde' no puede ser mayor que 'Hasta'.",
                    "Warning / Advertencia", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                return;
            }

            using var dbContext = _dbFactory.CreateDbContext();

            var from = dtFromTime.Value.Date;
            var to = dtToTime.Value.Date.AddDays(1).AddSeconds(-1);

            var records = dbContext.TbScanOuts
                .Where(s => s.ScanAt.HasValue)
                .Where(s => s.ClientId == _clientIp)
                .Where(s => s.ScanAt >= from && s.ScanAt <= to)
                .Select(s => new
                {
                    ScanAt = s.ScanAt!.Value,
                    Qty = s.Qty ?? 1,
                    PartNo = s.PartNo
                })
                .AsEnumerable()
                .Select(s => new ScanRecord(s.ScanAt, s.Qty, s.PartNo))
                .ToList();

            var summaries = BuildSummary(records);
            dvSummary.DataSource = summaries;

            // --- CẬP NHẬT KPI SUMMARY ---
            int totalScans = records.Count;
            int totalQty = records.Sum(r => r.Qty);
            int partCount = summaries.Select(s => s.PartNo).Where(p => !string.IsNullOrEmpty(p)).Distinct().Count();

            lblKpiTotalScanVal.Text = totalScans.ToString("N0");
            lblKpiTotalQtyVal.Text = totalQty.ToString("N0");
            lblKpiPartCountVal.Text = partCount.ToString("N0");

            // --- CẤU HÌNH HEADER CỘT (SONG NGỮ ANH - TÂY BAN NHA/MEXICO) ---
            if (dvSummary.Columns["Date"] != null)
            {
                dvSummary.Columns["Date"].HeaderText = "Date / Fecha";
                dvSummary.Columns["Date"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dvSummary.Columns["Date"].FillWeight = 90;
            }
            if (dvSummary.Columns["PartNo"] != null)
            {
                dvSummary.Columns["PartNo"].HeaderText = "Part No / Núm. Parte";
                dvSummary.Columns["PartNo"].DisplayIndex = 1;
                dvSummary.Columns["PartNo"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                dvSummary.Columns["PartNo"].FillWeight = 140;
            }
            if (dvSummary.Columns["Slot0730To1000"] != null)
            {
                dvSummary.Columns["Slot0730To1000"].HeaderText = "07:30 - 10:00";
                dvSummary.Columns["Slot0730To1000"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dvSummary.Columns["Slot1000To1230"] != null)
            {
                dvSummary.Columns["Slot1000To1230"].HeaderText = "10:00 - 12:30";
                dvSummary.Columns["Slot1000To1230"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dvSummary.Columns["Slot1230To1515"] != null)
            {
                dvSummary.Columns["Slot1230To1515"].HeaderText = "12:30 - 15:15";
                dvSummary.Columns["Slot1230To1515"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dvSummary.Columns["Slot1515To1730"] != null)
            {
                dvSummary.Columns["Slot1515To1730"].HeaderText = "15:15 - 17:30";
                dvSummary.Columns["Slot1515To1730"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dvSummary.Columns["Slot1730To2000"] != null)
            {
                dvSummary.Columns["Slot1730To2000"].HeaderText = "17:30 - 20:00";
                dvSummary.Columns["Slot1730To2000"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dvSummary.Columns["Slot2000To0730"] != null)
            {
                dvSummary.Columns["Slot2000To0730"].HeaderText = "20:00 - 07:30";
                dvSummary.Columns["Slot2000To0730"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dvSummary.Columns["Total"] != null)
            {
                dvSummary.Columns["Total"].HeaderText = "TOTAL / TOTAL";
                dvSummary.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dvSummary.Columns["Total"].FillWeight = 110;
            }

            // Dòng xen kẽ màu sáng dễ nhìn
            dvSummary.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void DvSummary_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string columnName = dvSummary.Columns[e.ColumnIndex].Name;

            // Highlight cột TOTAL bằng màu xanh bạc hà dịu nhẹ và chữ in đậm
            if (columnName == "Total")
            {
                e.CellStyle.BackColor = Color.FromArgb(236, 253, 245); // Light Emerald Mint
                e.CellStyle.ForeColor = Color.FromArgb(4, 120, 87);    // Deep Green
                e.CellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            }
            // Làm mờ các số 0 để các con số sản lượng nổi bật hơn
            else if (e.Value is int intVal && intVal == 0 && columnName != "PartNo" && columnName != "Date")
            {
                e.CellStyle.ForeColor = Color.FromArgb(203, 213, 225); // Muted Silver
            }
        }

        private List<SummaryRow> BuildSummary(IEnumerable<ScanRecord> records)
        {
            var result = new Dictionary<(DateOnly, string), SummaryRow>();

            foreach (var record in records)
            {
                var slotInfo = GetSlotInfo(record.ScanAt);
                string pNo = record.PartNo ?? "";

                var key = (slotInfo.Day, pNo);

                if (!result.TryGetValue(key, out var row))
                {
                    row = new SummaryRow(slotInfo.Day, pNo);
                    result[key] = row;
                }

                var qty = record.Qty;
                switch (slotInfo.Slot)
                {
                    case TimeSlot.Slot0730To1000:
                        row.Slot0730To1000 += qty;
                        break;
                    case TimeSlot.Slot1000To1230:
                        row.Slot1000To1230 += qty;
                        break;
                    case TimeSlot.Slot1230To1515:
                        row.Slot1230To1515 += qty;
                        break;
                    case TimeSlot.Slot1515To1730:
                        row.Slot1515To1730 += qty;
                        break;
                    case TimeSlot.Slot1730To2000:
                        row.Slot1730To2000 += qty;
                        break;
                    case TimeSlot.Slot2000To0730:
                        row.Slot2000To0730 += qty;
                        break;
                }
            }

            return result.Values
                .OrderBy(r => r.Date)
                .ThenBy(r => r.PartNo)
                .Select(r => r.WithTotal())
                .ToList();
        }

        private (DateOnly Day, TimeSlot Slot) GetSlotInfo(DateTime scanTime)
        {
            var timeOfDay = scanTime.TimeOfDay;
            var slotDay = DateOnly.FromDateTime(scanTime);
            var start0730 = new TimeSpan(7, 30, 0);

            if (timeOfDay < start0730)
            {
                slotDay = slotDay.AddDays(-1);
            }

            if (timeOfDay >= start0730 && timeOfDay < new TimeSpan(10, 0, 0))
                return (slotDay, TimeSlot.Slot0730To1000);
            if (timeOfDay >= new TimeSpan(10, 0, 0) && timeOfDay < new TimeSpan(12, 30, 0))
                return (slotDay, TimeSlot.Slot1000To1230);
            if (timeOfDay >= new TimeSpan(12, 30, 0) && timeOfDay < new TimeSpan(15, 15, 0))
                return (slotDay, TimeSlot.Slot1230To1515);
            if (timeOfDay >= new TimeSpan(15, 15, 0) && timeOfDay < new TimeSpan(17, 30, 0))
                return (slotDay, TimeSlot.Slot1515To1730);
            if (timeOfDay >= new TimeSpan(17, 30, 0) && timeOfDay < new TimeSpan(20, 0, 0))
                return (slotDay, TimeSlot.Slot1730To2000);

            return (slotDay, TimeSlot.Slot2000To0730);
        }

        private enum TimeSlot
        {
            Slot0730To1000,
            Slot1000To1230,
            Slot1230To1515,
            Slot1515To1730,
            Slot1730To2000,
            Slot2000To0730
        }

        private class SummaryRow
        {
            public SummaryRow(DateOnly date, string partNo)
            {
                Date = date.ToString("yyyy-MM-dd");
                PartNo = partNo;
            }

            public string Date { get; set; }
            public string PartNo { get; set; }
            public int Slot0730To1000 { get; set; }
            public int Slot1000To1230 { get; set; }
            public int Slot1230To1515 { get; set; }
            public int Slot1515To1730 { get; set; }
            public int Slot1730To2000 { get; set; }
            public int Slot2000To0730 { get; set; }
            public int Total { get; set; }

            public SummaryRow WithTotal()
            {
                Total = Slot0730To1000 + Slot1000To1230 + Slot1230To1515 + Slot1515To1730 + Slot1730To2000 + Slot2000To0730;
                return this;
            }
        }

        private readonly record struct ScanRecord(DateTime ScanAt, int Qty, string PartNo);
    }
}

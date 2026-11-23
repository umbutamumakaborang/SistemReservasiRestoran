using System.Drawing;
using System.Windows.Forms;

namespace RestoranFormsApp
{
    public static class UIHelper
    {
        public static DataGridView BuatGridStandar(Point lokasi, Size ukuran)
        {
            var dgv = new DataGridView
            {
                Location = lokasi,
                Size = ukuran,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = true,
                RowHeadersWidth = 28,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                Font = new Font("Segoe UI", 9.5f)
            };

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(228, 234, 244);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(25, 35, 50);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 30;
            dgv.RowTemplate.Height = 28;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 115, 205);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            return dgv;
        }

        public static Button BuatTombolUtama(string teks, Point lokasi, Size ukuran, AnchorStyles anchor)
        {
            var btn = new Button
            {
                Text = teks,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = lokasi,
                Size = ukuran,
                Anchor = anchor,
                BackColor = Color.FromArgb(198, 226, 255),
                ForeColor = Color.FromArgb(15, 45, 90),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(70, 130, 210);
            return btn;
        }

        public static Button BuatTombolSekunder(string teks, Point lokasi, Size ukuran, AnchorStyles anchor)
        {
            var btn = new Button
            {
                Text = teks,
                Font = new Font("Segoe UI", 9.5f),
                Location = lokasi,
                Size = ukuran,
                Anchor = anchor,
                BackColor = Color.FromArgb(228, 230, 234),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.Silver;
            return btn;
        }
    }
}

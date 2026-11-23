using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ReservasiLibrary;

namespace RestoranFormsApp
{
    public class FormPembatalanList : Form
    {
        private DataGridView dgvCancel;
        private ToolStripStatusLabel lblStatus;

        public FormPembatalanList()
        {
            this.Text = "Riwayat Pembatalan - Sistem Informasi Reservasi Restoran";
            this.Size = new Size(840, 490);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 246, 248);

            Label lblJudul = new Label { Text = "Riwayat Pembatalan Reservasi", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(30, 64, 115), Location = new Point(20, 14), AutoSize = true };

            dgvCancel = UIHelper.BuatGridStandar(new Point(20, 62), new Size(784, 305));
            dgvCancel.Columns.Add("IdCancel", "ID Cancel");
            dgvCancel.Columns["IdCancel"].FillWeight = 11;
            dgvCancel.Columns.Add("IdBooking", "ID Booking");
            dgvCancel.Columns["IdBooking"].FillWeight = 12;
            dgvCancel.Columns.Add("Pelanggan", "Pelanggan");
            dgvCancel.Columns["Pelanggan"].FillWeight = 20;
            dgvCancel.Columns.Add("Waktu", "Waktu Batal");
            dgvCancel.Columns["Waktu"].FillWeight = 18;
            dgvCancel.Columns.Add("Alasan", "Alasan");
            dgvCancel.Columns["Alasan"].FillWeight = 25;
            dgvCancel.Columns.Add("Admin", "Admin PJ");
            dgvCancel.Columns["Admin"].FillWeight = 14;
            dgvCancel.SelectionChanged += (s, e) => PerbaruiStatus();

            int y = 382;
            Button btnTambah = UIHelper.BuatTombolUtama("Tambah", new Point(20, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnUbah = UIHelper.BuatTombolSekunder("Ubah", new Point(113, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnHapus = UIHelper.BuatTombolSekunder("Hapus (Pop)", new Point(206, y), new Size(95, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnRefresh = UIHelper.BuatTombolSekunder("Refresh", new Point(626, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Right);
            Button btnTutup = UIHelper.BuatTombolSekunder("Tutup", new Point(719, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Right);

            btnTambah.Click += (s, e) => { if (new FormPembatalanEntri(null).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(); } };
            btnUbah.Click += (s, e) =>
            {
                if (dgvCancel.SelectedRows.Count == 0) return;
                var c = (Pembatalan)dgvCancel.SelectedRows[0].Tag;
                if (new FormPembatalanEntri(c).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(); }
            };
            btnHapus.Click += (s, e) =>
            {
                if (dgvCancel.SelectedRows.Count == 0) return;
                var c = (Pembatalan)dgvCancel.SelectedRows[0].Tag;
                if (MessageBox.Show($"Hapus riwayat cancel ID {c.IdCancel}?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataRepository.ListPembatalan.Remove(c);
                    DataRepository.SimpanSemuaData();
                    MuatData();
                }
            };
            btnRefresh.Click += (s, e) => MuatData();
            btnTutup.Click += (s, e) => this.Close();

            StatusStrip statusStrip = new StatusStrip { BackColor = Color.FromArgb(240, 240, 240), SizingGrip = false };
            lblStatus = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9f) };
            statusStrip.Items.Add(lblStatus);

            this.Controls.AddRange(new Control[] { lblJudul, dgvCancel, btnTambah, btnUbah, btnHapus, btnRefresh, btnTutup, statusStrip });
            MuatData();
        }

        private void MuatData()
        {
            dgvCancel.Rows.Clear();
            foreach (var c in DataRepository.ListPembatalan)
            {
                int idx = dgvCancel.Rows.Add(c.IdCancel, c.IdBooking, c.NamaPelanggan, c.WaktuCancel, c.AlasanBatal, c.AdminPenanggungJawab);
                dgvCancel.Rows[idx].Tag = c;
            }
            if (dgvCancel.Rows.Count > 0) dgvCancel.Rows[0].Selected = true;
            PerbaruiStatus();
        }

        private void PerbaruiStatus()
        {
            lblStatus.Text = $"Total data: {dgvCancel.Rows.Count}   |   Baris terpilih: {dgvCancel.SelectedRows.Count}";
        }
    }

    public class FormPembatalanEntri : Form
    {
        private TextBox txtIdCancel, txtWaktu, txtAlasan, txtAdmin;
        private ComboBox cmbBooking;
        private Pembatalan cancelEdit;

        public FormPembatalanEntri(Pembatalan editData)
        {
            this.cancelEdit = editData;
            this.Text = editData == null ? "Tambah Pembatalan Manual" : "Ubah Data Pembatalan";
            this.Size = new Size(440, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label l1 = new Label { Text = "ID Cancel:", Location = new Point(25, 25), AutoSize = true };
            txtIdCancel = new TextBox { Location = new Point(155, 22), Width = 100, ReadOnly = true };
            Label l2 = new Label { Text = "Pilih Booking:", Location = new Point(25, 65), AutoSize = true };
            cmbBooking = new ComboBox { Location = new Point(155, 62), Width = 235, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var b in DataRepository.ListBooking) cmbBooking.Items.Add($"{b.IdBooking} - {b.NamaPelanggan}");
            if (cmbBooking.Items.Count > 0) cmbBooking.SelectedIndex = 0;

            Label l3 = new Label { Text = "Waktu Batal:", Location = new Point(25, 105), AutoSize = true };
            txtWaktu = new TextBox { Location = new Point(155, 102), Width = 160, Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm") };
            Label l4 = new Label { Text = "Alasan Batal:", Location = new Point(25, 145), AutoSize = true };
            txtAlasan = new TextBox { Location = new Point(155, 142), Width = 235, Text = "Permintaan Pelanggan" };
            Label l5 = new Label { Text = "Admin PJ:", Location = new Point(25, 185), AutoSize = true };
            txtAdmin = new TextBox { Location = new Point(155, 182), Width = 160, Text = "admin" };

            Button btnSimpan = UIHelper.BuatTombolUtama("Simpan", new Point(155, 230), new Size(100, 30), AnchorStyles.Top | AnchorStyles.Left);
            Button btnBatal = UIHelper.BuatTombolSekunder("Batal", new Point(275, 230), new Size(100, 30), AnchorStyles.Top | AnchorStyles.Left);

            if (editData == null)
                txtIdCancel.Text = (DataRepository.ListPembatalan.Count > 0 ? DataRepository.ListPembatalan.Max(x => x.IdCancel) + 1 : 1).ToString();
            else
            {
                txtIdCancel.Text = editData.IdCancel.ToString();
                txtWaktu.Text = editData.WaktuCancel;
                txtAlasan.Text = editData.AlasanBatal;
                txtAdmin.Text = editData.AdminPenanggungJawab;
                for (int i = 0; i < cmbBooking.Items.Count; i++)
                    if (cmbBooking.Items[i].ToString().StartsWith(editData.IdBooking + " -")) cmbBooking.SelectedIndex = i;
            }

            btnSimpan.Click += (s, e) =>
            {
                if (cmbBooking.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtAlasan.Text)) { MessageBox.Show("Lengkapi ID Booking dan Alasan!"); return; }
                var parts = cmbBooking.SelectedItem.ToString().Split('-');
                int idB = int.Parse(parts[0].Trim());
                string namaP = parts[1].Trim();
                var dataBook = DataRepository.ListBooking.FirstOrDefault(x => x.IdBooking == idB);
                if (dataBook != null) dataBook.StatusBooking = "Dibatalkan";

                if (cancelEdit == null)
                    DataRepository.ListPembatalan.Insert(0, new Pembatalan { IdCancel = int.Parse(txtIdCancel.Text), IdBooking = idB, NamaPelanggan = namaP, WaktuCancel = txtWaktu.Text.Trim(), AlasanBatal = txtAlasan.Text.Trim(), AdminPenanggungJawab = txtAdmin.Text.Trim() });
                else
                {
                    cancelEdit.IdBooking = idB; cancelEdit.NamaPelanggan = namaP; cancelEdit.WaktuCancel = txtWaktu.Text.Trim();
                    cancelEdit.AlasanBatal = txtAlasan.Text.Trim(); cancelEdit.AdminPenanggungJawab = txtAdmin.Text.Trim();
                }
                this.DialogResult = DialogResult.OK;
            };
            btnBatal.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { l1, txtIdCancel, l2, cmbBooking, l3, txtWaktu, l4, txtAlasan, l5, txtAdmin, btnSimpan, btnBatal });
        }
    }
}

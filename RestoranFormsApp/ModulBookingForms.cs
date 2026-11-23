using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ReservasiLibrary;

namespace RestoranFormsApp
{
    public class FormBookingList : Form
    {
        private DataGridView dgvBooking;
        private ToolStripStatusLabel lblStatus;
        private bool isAdmin;

        public FormBookingList(bool isAdminRole)
        {
            this.isAdmin = isAdminRole;
            this.Text = "Antrean Reservasi / Booking - Sistem Informasi Reservasi Restoran";
            this.Size = new Size(920, 490);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 246, 248);

            Label lblJudul = new Label { Text = "Antrean Reservasi / Booking", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(30, 64, 115), Location = new Point(20, 14), AutoSize = true };

            dgvBooking = UIHelper.BuatGridStandar(new Point(20, 62), new Size(864, 305));
            dgvBooking.Columns.Add("IdBooking", "ID");
            dgvBooking.Columns["IdBooking"].FillWeight = 8;
            dgvBooking.Columns.Add("Pelanggan", "Pelanggan");
            dgvBooking.Columns["Pelanggan"].FillWeight = 18;
            dgvBooking.Columns.Add("Restoran", "Restoran");
            dgvBooking.Columns["Restoran"].FillWeight = 20;
            dgvBooking.Columns.Add("Menu", "Paket Menu");
            dgvBooking.Columns["Menu"].FillWeight = 20;
            dgvBooking.Columns.Add("Jam", "Jam");
            dgvBooking.Columns["Jam"].FillWeight = 9;
            dgvBooking.Columns.Add("Kursi", "Kursi");
            dgvBooking.Columns["Kursi"].FillWeight = 8;
            dgvBooking.Columns.Add("Total", "Total Bayar");
            dgvBooking.Columns["Total"].FillWeight = 13;
            dgvBooking.Columns.Add("Status", "Status");
            dgvBooking.Columns["Status"].FillWeight = 11;
            dgvBooking.SelectionChanged += (s, e) => PerbaruiStatus();

            int y = 382;
            Button btnTambah = UIHelper.BuatTombolUtama("Tambah", new Point(20, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnUbah = UIHelper.BuatTombolSekunder("Ubah", new Point(113, y), new Size(80, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnHapus = UIHelper.BuatTombolSekunder("Hapus", new Point(201, y), new Size(80, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnDequeue = UIHelper.BuatTombolSekunder("Proses Bayar", new Point(295, y), new Size(165, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            btnDequeue.BackColor = Color.FromArgb(209, 242, 224);
            Button btnCancel = UIHelper.BuatTombolSekunder("Batal Terakhir", new Point(468, y), new Size(155, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            btnCancel.BackColor = Color.FromArgb(250, 219, 216);
            Button btnTutup = UIHelper.BuatTombolSekunder("Tutup", new Point(799, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Right);

            btnTambah.Click += (s, e) => { if (new FormBookingEntri(null).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(); } };
            btnUbah.Click += (s, e) =>
            {
                if (dgvBooking.SelectedRows.Count == 0) return;
                var b = (Booking)dgvBooking.SelectedRows[0].Tag;
                if (new FormBookingEntri(b).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(); }
            };
            btnHapus.Click += (s, e) =>
            {
                if (dgvBooking.SelectedRows.Count == 0) return;
                var b = (Booking)dgvBooking.SelectedRows[0].Tag;
                if (MessageBox.Show($"Hapus booking ID {b.IdBooking}?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataRepository.ListBooking.Remove(b);
                    DataRepository.SimpanSemuaData();
                    MuatData();
                }
            };
            btnDequeue.Click += (s, e) =>
            {
                var pertama = DataRepository.ListBooking.FirstOrDefault(x => x.StatusBooking == "Antrean");
                if (pertama == null) { MessageBox.Show("Tidak ada antrean aktif!"); return; }
                pertama.StatusBooking = "Lunas";
                var menu = DataRepository.ListMenu.FirstOrDefault(m => m.IdMenu == pertama.IdMenu);
                if (menu != null) menu.Terjual += pertama.JumlahKursi;
                DataRepository.SimpanSemuaData();
                MuatData();
                MessageBox.Show($"nPembayaran ID {pertama.IdBooking} ({pertama.NamaPelanggan}) -> LUNAS!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            btnCancel.Click += (s, e) =>
            {
                var terakhir = DataRepository.ListBooking.LastOrDefault(x => x.StatusBooking == "Antrean");
                if (terakhir == null) { MessageBox.Show("Tidak ada antrean aktif untuk dibatalkan!"); return; }
                terakhir.StatusBooking = "Dibatalkan";
                int nextId = DataRepository.ListPembatalan.Count > 0 ? DataRepository.ListPembatalan.Max(x => x.IdCancel) + 1 : 1;
                DataRepository.ListPembatalan.Insert(0, new Pembatalan
                {
                    IdCancel = nextId,
                    IdBooking = terakhir.IdBooking,
                    NamaPelanggan = terakhir.NamaPelanggan,
                    WaktuCancel = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    AlasanBatal = "Dibatalkan sebelum pembayaran",
                    AdminPenanggungJawab = isAdmin ? "admin" : "Self-Service"
                });
                DataRepository.SimpanSemuaData();
                MuatData();
                MessageBox.Show($"Booking ID {terakhir.IdBooking} dibatalkan dan masuk ke Stack Pembatalan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            btnTutup.Click += (s, e) => this.Close();

            StatusStrip statusStrip = new StatusStrip { BackColor = Color.FromArgb(240, 240, 240), SizingGrip = false };
            lblStatus = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9f) };
            statusStrip.Items.Add(lblStatus);

            this.Controls.AddRange(new Control[] { lblJudul, dgvBooking, btnTambah, btnUbah, btnHapus, btnDequeue, btnCancel, btnTutup, statusStrip });
            MuatData();
        }

        private void MuatData()
        {
            dgvBooking.Rows.Clear();
            foreach (var b in DataRepository.ListBooking)
            {
                int idx = dgvBooking.Rows.Add(b.IdBooking, b.NamaPelanggan, DataRepository.GetNamaRestoran(b.IdRestoran), DataRepository.GetNamaMenu(b.IdMenu), b.WaktuBooking, b.JumlahKursi, "Rp" + b.TotalBayar.ToString("N0"), b.StatusBooking);
                dgvBooking.Rows[idx].Tag = b;
            }
            if (dgvBooking.Rows.Count > 0) dgvBooking.Rows[0].Selected = true;
            PerbaruiStatus();
        }

        private void PerbaruiStatus()
        {
            lblStatus.Text = $"Total data: {dgvBooking.Rows.Count}   |   Baris terpilih: {dgvBooking.SelectedRows.Count}";
        }
    }

    public class FormBookingEntri : Form
    {
        private TextBox txtId, txtPelanggan, txtWaktu, txtTotal;
        private ComboBox cmbResto, cmbMenu, cmbStatus;
        private NumericUpDown numKursi;
        private Booking bookingEdit;

        public FormBookingEntri(Booking editData)
        {
            this.bookingEdit = editData;
            this.Text = editData == null ? "Tambah Reservasi Booking" : "Ubah Data Booking";
            this.Size = new Size(450, 410);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label l1 = new Label { Text = "ID Booking:", Location = new Point(25, 25), AutoSize = true };
            txtId = new TextBox { Location = new Point(160, 22), Width = 100, ReadOnly = true };
            Label l2 = new Label { Text = "Pilih Restoran:", Location = new Point(25, 65), AutoSize = true };
            cmbResto = new ComboBox { Location = new Point(160, 62), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            Label l3 = new Label { Text = "Pilih Paket Menu:", Location = new Point(25, 105), AutoSize = true };
            cmbMenu = new ComboBox { Location = new Point(160, 102), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            Label l4 = new Label { Text = "Nama Pelanggan:", Location = new Point(25, 145), AutoSize = true };
            txtPelanggan = new TextBox { Location = new Point(160, 142), Width = 240 };
            Label l5 = new Label { Text = "Jam Booking:", Location = new Point(25, 185), AutoSize = true };
            txtWaktu = new TextBox { Location = new Point(160, 182), Width = 120, Text = "19:00" };
            Label l6 = new Label { Text = "Jumlah Kursi:", Location = new Point(25, 225), AutoSize = true };
            numKursi = new NumericUpDown { Location = new Point(160, 222), Width = 100, Minimum = 1, Maximum = 100, Value = 2 };
            Label l7 = new Label { Text = "Total Tagihan (Rp):", Location = new Point(25, 265), AutoSize = true };
            txtTotal = new TextBox { Location = new Point(160, 262), Width = 150, ReadOnly = true };
            Label l8 = new Label { Text = "Status Booking:", Location = new Point(25, 305), AutoSize = true };
            cmbStatus = new ComboBox { Location = new Point(160, 302), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new object[] { "Antrean", "Lunas", "Dibatalkan" });
            cmbStatus.SelectedIndex = 0;

            foreach (var r in DataRepository.ListRestoran) cmbResto.Items.Add($"{r.IdRestoran} - {r.NamaRestoran}");
            cmbResto.SelectedIndexChanged += (s, e) => MuatMenuSesuaiResto();
            cmbMenu.SelectedIndexChanged += (s, e) => HitungTotal();
            numKursi.ValueChanged += (s, e) => HitungTotal();
            if (cmbResto.Items.Count > 0) cmbResto.SelectedIndex = 0;

            if (editData == null)
                txtId.Text = (DataRepository.ListBooking.Count > 0 ? DataRepository.ListBooking.Max(x => x.IdBooking) + 1 : 101).ToString();
            else
            {
                txtId.Text = editData.IdBooking.ToString();
                txtPelanggan.Text = editData.NamaPelanggan;
                txtWaktu.Text = editData.WaktuBooking;
                numKursi.Value = editData.JumlahKursi;
                cmbStatus.SelectedItem = editData.StatusBooking;
                for (int i = 0; i < cmbResto.Items.Count; i++)
                    if (cmbResto.Items[i].ToString().StartsWith(editData.IdRestoran + " -")) cmbResto.SelectedIndex = i;
                for (int i = 0; i < cmbMenu.Items.Count; i++)
                    if (cmbMenu.Items[i].ToString().StartsWith(editData.IdMenu + " -")) cmbMenu.SelectedIndex = i;
            }

            Button btnSimpan = UIHelper.BuatTombolUtama("Simpan", new Point(160, 340), new Size(100, 30), AnchorStyles.Top | AnchorStyles.Left);
            Button btnBatal = UIHelper.BuatTombolSekunder("Batal", new Point(280, 340), new Size(100, 30), AnchorStyles.Top | AnchorStyles.Left);
            btnSimpan.Click += (s, e) =>
            {
                if (cmbResto.SelectedIndex < 0 || cmbMenu.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtPelanggan.Text)) { MessageBox.Show("Lengkapi semua data!"); return; }
                int idR = int.Parse(cmbResto.SelectedItem.ToString().Split('-')[0].Trim());
                int idM = int.Parse(cmbMenu.SelectedItem.ToString().Split('-')[0].Trim());
                int total = int.Parse(txtTotal.Text);
                if (bookingEdit == null)
                    DataRepository.ListBooking.Add(new Booking { IdBooking = int.Parse(txtId.Text), IdRestoran = idR, IdMenu = idM, NamaPelanggan = txtPelanggan.Text.Trim(), WaktuBooking = txtWaktu.Text.Trim(), JumlahKursi = (int)numKursi.Value, TotalBayar = total, StatusBooking = cmbStatus.SelectedItem.ToString() });
                else
                {
                    bookingEdit.IdRestoran = idR; bookingEdit.IdMenu = idM; bookingEdit.NamaPelanggan = txtPelanggan.Text.Trim();
                    bookingEdit.WaktuBooking = txtWaktu.Text.Trim(); bookingEdit.JumlahKursi = (int)numKursi.Value;
                    bookingEdit.TotalBayar = total; bookingEdit.StatusBooking = cmbStatus.SelectedItem.ToString();
                }
                this.DialogResult = DialogResult.OK;
            };
            btnBatal.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { l1, txtId, l2, cmbResto, l3, cmbMenu, l4, txtPelanggan, l5, txtWaktu, l6, numKursi, l7, txtTotal, l8, cmbStatus, btnSimpan, btnBatal });
            HitungTotal();
        }

        private void MuatMenuSesuaiResto()
        {
            cmbMenu.Items.Clear();
            if (cmbResto.SelectedIndex < 0) return;
            int idR = int.Parse(cmbResto.SelectedItem.ToString().Split('-')[0].Trim());
            foreach (var m in DataRepository.ListMenu.Where(x => x.IdRestoran == idR))
                cmbMenu.Items.Add($"{m.IdMenu} - {m.NamaMenu} (Rp{m.Harga})");
            if (cmbMenu.Items.Count > 0) cmbMenu.SelectedIndex = 0;
            HitungTotal();
        }

        private void HitungTotal()
        {
            if (cmbMenu.SelectedIndex < 0) { txtTotal.Text = "0"; return; }
            int idM = int.Parse(cmbMenu.SelectedItem.ToString().Split('-')[0].Trim());
            var menu = DataRepository.ListMenu.FirstOrDefault(x => x.IdMenu == idM);
            txtTotal.Text = ((menu != null ? menu.Harga : 0) * (int)numKursi.Value).ToString();
        }
    }
}

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ReservasiLibrary;

namespace RestoranFormsApp
{
    public class FormRestoranList : Form
    {
        private DataGridView dgvRestoran;
        private TextBox txtCari;
        private ToolStripStatusLabel lblStatus;

        public FormRestoranList()
        {
            this.Text = "Data Restoran - Sistem Informasi Reservasi Restoran";
            this.Size = new Size(820, 490);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 246, 248);

            Label lblJudul = new Label { Text = "Data Restoran", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(30, 64, 115), Location = new Point(20, 14), AutoSize = true };
            Label lblCari = new Label { Text = "Cari:", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Location = new Point(20, 56), AutoSize = true };
            txtCari = new TextBox { Font = new Font("Segoe UI", 9.5f), Location = new Point(62, 53), Size = new Size(235, 25) };
            Button btnCari = UIHelper.BuatTombolUtama("Cari", new Point(307, 52), new Size(75, 27), AnchorStyles.Top | AnchorStyles.Left);
            btnCari.Click += (s, e) => MuatData(txtCari.Text);
            txtCari.TextChanged += (s, e) => MuatData(txtCari.Text);

            dgvRestoran = UIHelper.BuatGridStandar(new Point(20, 92), new Size(764, 275));
            dgvRestoran.Columns.Add("IdRestoran", "ID");
            dgvRestoran.Columns["IdRestoran"].FillWeight = 10;
            dgvRestoran.Columns.Add("NamaRestoran", "Nama Restoran");
            dgvRestoran.Columns["NamaRestoran"].FillWeight = 26;
            dgvRestoran.Columns.Add("Alamat", "Alamat");
            dgvRestoran.Columns["Alamat"].FillWeight = 32;
            dgvRestoran.Columns.Add("NoTelp", "No. Telepon");
            dgvRestoran.Columns["NoTelp"].FillWeight = 18;
            dgvRestoran.Columns.Add("Kapasitas", "Kapasitas Meja");
            dgvRestoran.Columns["Kapasitas"].FillWeight = 14;
            dgvRestoran.SelectionChanged += (s, e) => PerbaruiStatus();

            int y = 382;
            Button btnTambah = UIHelper.BuatTombolUtama("Tambah", new Point(20, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnUbah = UIHelper.BuatTombolSekunder("Ubah", new Point(113, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnHapus = UIHelper.BuatTombolSekunder("Hapus", new Point(206, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnRefresh = UIHelper.BuatTombolSekunder("Refresh", new Point(606, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Right);
            Button btnTutup = UIHelper.BuatTombolSekunder("Tutup", new Point(699, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Right);

            btnTambah.Click += (s, e) => { if (new FormRestoranEntri(null).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(txtCari.Text); } };
            btnUbah.Click += (s, e) =>
            {
                if (dgvRestoran.SelectedRows.Count == 0) return;
                var r = (Restoran)dgvRestoran.SelectedRows[0].Tag;
                if (new FormRestoranEntri(r).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(txtCari.Text); }
            };
            btnHapus.Click += (s, e) =>
            {
                if (dgvRestoran.SelectedRows.Count == 0) return;
                var r = (Restoran)dgvRestoran.SelectedRows[0].Tag;
                if (MessageBox.Show($"Hapus restoran '{r.NamaRestoran}'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataRepository.ListRestoran.Remove(r);
                    DataRepository.SimpanSemuaData();
                    MuatData(txtCari.Text);
                }
            };
            btnRefresh.Click += (s, e) => { txtCari.Clear(); MuatData(""); };
            btnTutup.Click += (s, e) => this.Close();

            StatusStrip statusStrip = new StatusStrip { BackColor = Color.FromArgb(240, 240, 240), SizingGrip = false };
            lblStatus = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9f) };
            statusStrip.Items.Add(lblStatus);

            this.Controls.AddRange(new Control[] { lblJudul, lblCari, txtCari, btnCari, dgvRestoran, btnTambah, btnUbah, btnHapus, btnRefresh, btnTutup, statusStrip });
            MuatData("");
        }

        private void MuatData(string keyword)
        {
            dgvRestoran.Rows.Clear();
            var data = DataRepository.ListRestoran.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(keyword))
                data = data.Where(r => r.NamaRestoran.ToLower().Contains(keyword.Trim().ToLower()));
            foreach (var r in data)
            {
                int idx = dgvRestoran.Rows.Add(r.IdRestoran, r.NamaRestoran, r.Alamat, r.NoTelp, r.KapasitasMeja);
                dgvRestoran.Rows[idx].Tag = r;
            }
            if (dgvRestoran.Rows.Count > 0) dgvRestoran.Rows[0].Selected = true;
            PerbaruiStatus();
        }

        private void PerbaruiStatus()
        {
            lblStatus.Text = $"Total data: {dgvRestoran.Rows.Count}   |   Baris terpilih: {dgvRestoran.SelectedRows.Count}";
        }
    }

    public class FormRestoranEntri : Form
    {
        private TextBox txtId, txtNama, txtAlamat, txtTelp;
        private NumericUpDown numKapasitas;
        private Restoran restoEdit;

        public FormRestoranEntri(Restoran editData)
        {
            this.restoEdit = editData;
            this.Text = editData == null ? "Tambah Data Restoran" : "Ubah Data Restoran";
            this.Size = new Size(420, 310);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label l1 = new Label { Text = "ID Restoran:", Location = new Point(25, 25), AutoSize = true };
            txtId = new TextBox { Location = new Point(145, 22), Width = 100, ReadOnly = true };
            Label l2 = new Label { Text = "Nama Restoran:", Location = new Point(25, 65), AutoSize = true };
            txtNama = new TextBox { Location = new Point(145, 62), Width = 220 };
            Label l3 = new Label { Text = "Alamat:", Location = new Point(25, 105), AutoSize = true };
            txtAlamat = new TextBox { Location = new Point(145, 102), Width = 220 };
            Label l4 = new Label { Text = "No. Telepon:", Location = new Point(25, 145), AutoSize = true };
            txtTelp = new TextBox { Location = new Point(145, 142), Width = 160 };
            Label l5 = new Label { Text = "Kapasitas Meja:", Location = new Point(25, 185), AutoSize = true };
            numKapasitas = new NumericUpDown { Location = new Point(145, 182), Width = 100, Minimum = 1, Maximum = 500, Value = 20 };

            Button btnSimpan = UIHelper.BuatTombolUtama("Simpan", new Point(145, 225), new Size(100, 30), AnchorStyles.Top | AnchorStyles.Left);
            Button btnBatal = UIHelper.BuatTombolSekunder("Batal", new Point(265, 225), new Size(100, 30), AnchorStyles.Top | AnchorStyles.Left);

            if (editData == null)
                txtId.Text = (DataRepository.ListRestoran.Count > 0 ? DataRepository.ListRestoran.Max(x => x.IdRestoran) + 1 : 1).ToString();
            else
            {
                txtId.Text = editData.IdRestoran.ToString();
                txtNama.Text = editData.NamaRestoran;
                txtAlamat.Text = editData.Alamat;
                txtTelp.Text = editData.NoTelp;
                numKapasitas.Value = editData.KapasitasMeja;
            }

            btnSimpan.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNama.Text)) { MessageBox.Show("Nama Restoran wajib diisi!"); return; }
                if (restoEdit == null)
                    DataRepository.ListRestoran.Add(new Restoran { IdRestoran = int.Parse(txtId.Text), NamaRestoran = txtNama.Text.Trim(), Alamat = txtAlamat.Text.Trim(), NoTelp = txtTelp.Text.Trim(), KapasitasMeja = (int)numKapasitas.Value });
                else
                {
                    restoEdit.NamaRestoran = txtNama.Text.Trim();
                    restoEdit.Alamat = txtAlamat.Text.Trim();
                    restoEdit.NoTelp = txtTelp.Text.Trim();
                    restoEdit.KapasitasMeja = (int)numKapasitas.Value;
                }
                this.DialogResult = DialogResult.OK;
            };
            btnBatal.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { l1, txtId, l2, txtNama, l3, txtAlamat, l4, txtTelp, l5, numKapasitas, btnSimpan, btnBatal });
        }
    }
}

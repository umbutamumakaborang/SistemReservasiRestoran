using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ReservasiLibrary;

namespace RestoranFormsApp
{
    public class FormPelangganList : Form
    {
        private TextBox txtCari;
        private Label lblPlaceholder;
        private DataGridView dgvPelanggan;
        private ToolStripStatusLabel lblStatusInfo;

        public FormPelangganList()
        {
            this.Text = "Data Pelanggan - Sistem Informasi Reservasi Restoran";
            this.Size = new Size(820, 490);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 246, 248);

            Label lblJudul = new Label
            {
                Text = "Data Pelanggan",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 115),
                Location = new Point(20, 14),
                AutoSize = true
            };

            Label lblCari = new Label { Text = "Cari:", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Location = new Point(20, 56), AutoSize = true };
            txtCari = new TextBox { Font = new Font("Segoe UI", 9.5f), Location = new Point(62, 53), Size = new Size(235, 25), BackColor = Color.White };

            lblPlaceholder = new Label
            {
                Text = "Nama / no. telepon...",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.Gray,
                BackColor = Color.White,
                Location = new Point(66, 56),
                AutoSize = true,
                Cursor = Cursors.IBeam
            };
            lblPlaceholder.Click += (s, e) => txtCari.Focus();
            txtCari.TextChanged += (s, e) => { lblPlaceholder.Visible = string.IsNullOrEmpty(txtCari.Text); MuatData(txtCari.Text); };

            Button btnCari = UIHelper.BuatTombolUtama("Cari", new Point(307, 52), new Size(75, 27), AnchorStyles.Top | AnchorStyles.Left);
            btnCari.Click += (s, e) => MuatData(txtCari.Text);

            dgvPelanggan = UIHelper.BuatGridStandar(new Point(20, 92), new Size(764, 275));
            dgvPelanggan.Columns.Add("IdPelanggan", "ID");
            dgvPelanggan.Columns["IdPelanggan"].FillWeight = 12;
            dgvPelanggan.Columns.Add("NamaPelanggan", "Nama Pelanggan");
            dgvPelanggan.Columns["NamaPelanggan"].FillWeight = 25;
            dgvPelanggan.Columns.Add("NoTelepon", "No. Telepon");
            dgvPelanggan.Columns["NoTelepon"].FillWeight = 22;
            dgvPelanggan.Columns.Add("Email", "Email");
            dgvPelanggan.Columns["Email"].FillWeight = 22;
            dgvPelanggan.Columns.Add("Catatan", "Catatan");
            dgvPelanggan.Columns["Catatan"].FillWeight = 19;
            dgvPelanggan.SelectionChanged += (s, e) => PerbaruiStatus();

            int y = 382;
            Button btnTambah = UIHelper.BuatTombolUtama("Tambah", new Point(20, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnUbah = UIHelper.BuatTombolSekunder("Ubah", new Point(113, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnHapus = UIHelper.BuatTombolSekunder("Hapus", new Point(206, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnRefresh = UIHelper.BuatTombolSekunder("Refresh", new Point(606, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Right);
            Button btnTutup = UIHelper.BuatTombolSekunder("Tutup", new Point(699, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Right);

            btnTambah.Click += (s, e) => { if (new FormPelangganEntri(null).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(txtCari.Text); } };
            btnUbah.Click += (s, e) =>
            {
                if (dgvPelanggan.SelectedRows.Count == 0) { MessageBox.Show("Pilih data terlebih dahulu."); return; }
                var p = (Pelanggan)dgvPelanggan.SelectedRows[0].Tag;
                if (new FormPelangganEntri(p).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(txtCari.Text); }
            };
            btnHapus.Click += (s, e) =>
            {
                if (dgvPelanggan.SelectedRows.Count == 0) { MessageBox.Show("Pilih data terlebih dahulu."); return; }
                var p = (Pelanggan)dgvPelanggan.SelectedRows[0].Tag;
                if (MessageBox.Show($"Hapus pelanggan '{p.NamaPelanggan}'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataRepository.ListPelanggan.Remove(p);
                    DataRepository.SimpanSemuaData();
                    MuatData(txtCari.Text);
                }
            };
            btnRefresh.Click += (s, e) => { txtCari.Clear(); MuatData(""); };
            btnTutup.Click += (s, e) => this.Close();

            StatusStrip statusStrip = new StatusStrip { BackColor = Color.FromArgb(240, 240, 240), SizingGrip = false };
            lblStatusInfo = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9f) };
            statusStrip.Items.Add(lblStatusInfo);

            this.Controls.AddRange(new Control[] { lblJudul, lblCari, lblPlaceholder, txtCari, btnCari, dgvPelanggan, btnTambah, btnUbah, btnHapus, btnRefresh, btnTutup, statusStrip });
            MuatData("");
        }

        private void MuatData(string keyword)
        {
            dgvPelanggan.Rows.Clear();
            IEnumerable<Pelanggan> data = DataRepository.ListPelanggan;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string k = keyword.Trim().ToLower();
                data = data.Where(p => p.NamaPelanggan.ToLower().Contains(k) || p.NoTelepon.ToLower().Contains(k));
            }
            foreach (var p in data)
            {
                int idx = dgvPelanggan.Rows.Add(p.IdPelanggan, p.NamaPelanggan, p.NoTelepon, p.Email, p.Catatan);
                dgvPelanggan.Rows[idx].Tag = p;
            }
            if (dgvPelanggan.Rows.Count > 0) dgvPelanggan.Rows[0].Selected = true;
            PerbaruiStatus();
        }

        private void PerbaruiStatus()
        {
            lblStatusInfo.Text = $"Total data: {dgvPelanggan.Rows.Count}   |   Baris terpilih: {dgvPelanggan.SelectedRows.Count}";
        }
    }

    public class FormPelangganEntri : Form
    {
        private TextBox txtId, txtNama, txtTelp, txtEmail, txtCatatan;
        private Pelanggan dataEdit;

        public FormPelangganEntri(Pelanggan edit)
        {
            this.dataEdit = edit;
            this.Text = edit == null ? "Tambah Data Pelanggan" : "Ubah Data Pelanggan";
            this.Size = new Size(400, 290);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label l1 = new Label { Text = "ID:", Location = new Point(25, 25), AutoSize = true };
            txtId = new TextBox { Location = new Point(140, 22), Width = 90, ReadOnly = true };
            Label l2 = new Label { Text = "Nama Pelanggan:", Location = new Point(25, 60), AutoSize = true };
            txtNama = new TextBox { Location = new Point(140, 57), Width = 215 };
            Label l3 = new Label { Text = "No. Telepon:", Location = new Point(25, 95), AutoSize = true };
            txtTelp = new TextBox { Location = new Point(140, 92), Width = 215 };
            Label l4 = new Label { Text = "Email:", Location = new Point(25, 130), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(140, 127), Width = 215 };
            Label l5 = new Label { Text = "Catatan:", Location = new Point(25, 165), AutoSize = true };
            txtCatatan = new TextBox { Location = new Point(140, 162), Width = 215 };

            Button btnSimpan = UIHelper.BuatTombolUtama("Simpan", new Point(140, 205), new Size(95, 30), AnchorStyles.Top | AnchorStyles.Left);
            Button btnBatal = UIHelper.BuatTombolSekunder("Batal", new Point(260, 205), new Size(95, 30), AnchorStyles.Top | AnchorStyles.Left);

            if (edit == null)
                txtId.Text = (DataRepository.ListPelanggan.Count > 0 ? DataRepository.ListPelanggan.Max(x => x.IdPelanggan) + 1 : 1).ToString();
            else
            {
                txtId.Text = edit.IdPelanggan.ToString();
                txtNama.Text = edit.NamaPelanggan;
                txtTelp.Text = edit.NoTelepon;
                txtEmail.Text = edit.Email;
                txtCatatan.Text = edit.Catatan;
            }

            btnSimpan.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNama.Text) || string.IsNullOrWhiteSpace(txtTelp.Text))
                {
                    MessageBox.Show("Nama dan No. Telepon wajib diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dataEdit == null)
                {
                    DataRepository.ListPelanggan.Add(new Pelanggan
                    {
                        IdPelanggan = int.Parse(txtId.Text),
                        NamaPelanggan = txtNama.Text.Trim(),
                        NoTelepon = txtTelp.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? "-" : txtEmail.Text.Trim(),
                        Catatan = string.IsNullOrWhiteSpace(txtCatatan.Text) ? "-" : txtCatatan.Text.Trim()
                    });
                }
                else
                {
                    dataEdit.NamaPelanggan = txtNama.Text.Trim();
                    dataEdit.NoTelepon = txtTelp.Text.Trim();
                    dataEdit.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? "-" : txtEmail.Text.Trim();
                    dataEdit.Catatan = string.IsNullOrWhiteSpace(txtCatatan.Text) ? "-" : txtCatatan.Text.Trim();
                }
                this.DialogResult = DialogResult.OK;
            };
            btnBatal.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { l1, txtId, l2, txtNama, l3, txtTelp, l4, txtEmail, l5, txtCatatan, btnSimpan, btnBatal });
        }
    }
}

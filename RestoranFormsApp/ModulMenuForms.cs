using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ReservasiLibrary;

namespace RestoranFormsApp
{
    public class FormMenuList : Form
    {
        private DataGridView dgvMenu;
        private ComboBox cmbFilterResto;
        private CheckBox chkBestSeller;
        private ToolStripStatusLabel lblStatus;

        public FormMenuList()
        {
            this.Text = "Katalog Menu & Best Seller - Sistem Informasi Reservasi Restoran";
            this.Size = new Size(840, 490);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 246, 248);

            Label lblJudul = new Label { Text = "Katalog Menu & Best Seller", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(30, 64, 115), Location = new Point(20, 14), AutoSize = true };
            Label lblFilter = new Label { Text = "Restoran:", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Location = new Point(20, 56), AutoSize = true };
            cmbFilterResto = new ComboBox { Location = new Point(92, 53), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            chkBestSeller = new CheckBox { Text = "Urutkan Best Seller (Bubble Sort Terlaris)", Location = new Point(335, 55), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };

            cmbFilterResto.SelectedIndexChanged += (s, e) => MuatData();
            chkBestSeller.CheckedChanged += (s, e) => MuatData();

            dgvMenu = UIHelper.BuatGridStandar(new Point(20, 92), new Size(784, 275));
            dgvMenu.Columns.Add("IdMenu", "ID");
            dgvMenu.Columns["IdMenu"].FillWeight = 8;
            dgvMenu.Columns.Add("Restoran", "Restoran");
            dgvMenu.Columns["Restoran"].FillWeight = 24;
            dgvMenu.Columns.Add("NamaMenu", "Nama Menu");
            dgvMenu.Columns["NamaMenu"].FillWeight = 26;
            dgvMenu.Columns.Add("Kategori", "Kategori");
            dgvMenu.Columns["Kategori"].FillWeight = 14;
            dgvMenu.Columns.Add("Harga", "Harga (Rp)");
            dgvMenu.Columns["Harga"].FillWeight = 15;
            dgvMenu.Columns.Add("Terjual", "Terjual");
            dgvMenu.Columns["Terjual"].FillWeight = 13;
            dgvMenu.SelectionChanged += (s, e) => PerbaruiStatus();

            int y = 382;
            Button btnTambah = UIHelper.BuatTombolUtama("Tambah", new Point(20, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnUbah = UIHelper.BuatTombolSekunder("Ubah", new Point(113, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnHapus = UIHelper.BuatTombolSekunder("Hapus", new Point(206, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Left);
            Button btnRefresh = UIHelper.BuatTombolSekunder("Refresh", new Point(626, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Right);
            Button btnTutup = UIHelper.BuatTombolSekunder("Tutup", new Point(719, y), new Size(85, 32), AnchorStyles.Bottom | AnchorStyles.Right);

            btnTambah.Click += (s, e) => { if (new FormMenuEntri(null).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(); } };
            btnUbah.Click += (s, e) =>
            {
                if (dgvMenu.SelectedRows.Count == 0) return;
                var m = (MenuResto)dgvMenu.SelectedRows[0].Tag;
                if (new FormMenuEntri(m).ShowDialog() == DialogResult.OK) { DataRepository.SimpanSemuaData(); MuatData(); }
            };
            btnHapus.Click += (s, e) =>
            {
                if (dgvMenu.SelectedRows.Count == 0) return;
                var m = (MenuResto)dgvMenu.SelectedRows[0].Tag;
                if (MessageBox.Show($"Hapus menu '{m.NamaMenu}'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataRepository.ListMenu.Remove(m);
                    DataRepository.SimpanSemuaData();
                    MuatData();
                }
            };
            btnRefresh.Click += (s, e) => MuatData();
            btnTutup.Click += (s, e) => this.Close();

            StatusStrip statusStrip = new StatusStrip { BackColor = Color.FromArgb(240, 240, 240), SizingGrip = false };
            lblStatus = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9f) };
            statusStrip.Items.Add(lblStatus);

            this.Controls.AddRange(new Control[] { lblJudul, lblFilter, cmbFilterResto, chkBestSeller, dgvMenu, btnTambah, btnUbah, btnHapus, btnRefresh, btnTutup, statusStrip });

            cmbFilterResto.Items.Add("0 - Semua Restoran");
            foreach (var r in DataRepository.ListRestoran) cmbFilterResto.Items.Add($"{r.IdRestoran} - {r.NamaRestoran}");
            cmbFilterResto.SelectedIndex = 0;
        }

        private void MuatData()
        {
            dgvMenu.Rows.Clear();
            List<MenuResto> data = new List<MenuResto>(DataRepository.ListMenu);
            if (cmbFilterResto.SelectedIndex > 0)
            {
                int idR = int.Parse(cmbFilterResto.SelectedItem.ToString().Split('-')[0].Trim());
                data = data.Where(m => m.IdRestoran == idR).ToList();
            }
            if (chkBestSeller.Checked) data = DataRepository.UrutkanMenuBestSeller(data);

            foreach (var m in data)
            {
                int idx = dgvMenu.Rows.Add(m.IdMenu, DataRepository.GetNamaRestoran(m.IdRestoran), m.NamaMenu, m.KategoriPaket, m.Harga.ToString("N0"), m.Terjual + " pcs");
                dgvMenu.Rows[idx].Tag = m;
            }
            if (dgvMenu.Rows.Count > 0) dgvMenu.Rows[0].Selected = true;
            PerbaruiStatus();
        }

        private void PerbaruiStatus()
        {
            lblStatus.Text = $"Total data: {dgvMenu.Rows.Count}   |   Baris terpilih: {dgvMenu.SelectedRows.Count}";
        }
    }

    public class FormMenuEntri : Form
    {
        private TextBox txtId, txtNama;
        private ComboBox cmbResto, cmbKategori;
        private NumericUpDown numHarga, numTerjual;
        private MenuResto menuEdit;

        public FormMenuEntri(MenuResto editData)
        {
            this.menuEdit = editData;
            this.Text = editData == null ? "Tambah Menu Baru" : "Ubah Menu Restoran";
            this.Size = new Size(430, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label l1 = new Label { Text = "ID Menu:", Location = new Point(25, 25), AutoSize = true };
            txtId = new TextBox { Location = new Point(150, 22), Width = 100, ReadOnly = true };
            Label l2 = new Label { Text = "Pilih Restoran:", Location = new Point(25, 65), AutoSize = true };
            cmbResto = new ComboBox { Location = new Point(150, 62), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var r in DataRepository.ListRestoran) cmbResto.Items.Add($"{r.IdRestoran} - {r.NamaRestoran}");
            if (cmbResto.Items.Count > 0) cmbResto.SelectedIndex = 0;

            Label l3 = new Label { Text = "Nama Menu:", Location = new Point(25, 105), AutoSize = true };
            txtNama = new TextBox { Location = new Point(150, 102), Width = 230 };
            Label l4 = new Label { Text = "Kategori Paket:", Location = new Point(25, 145), AutoSize = true };
            cmbKategori = new ComboBox { Location = new Point(150, 142), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbKategori.Items.AddRange(new object[] { "Reguler", "AYCE" });
            cmbKategori.SelectedIndex = 0;

            Label l5 = new Label { Text = "Harga (Rp):", Location = new Point(25, 185), AutoSize = true };
            numHarga = new NumericUpDown { Location = new Point(150, 182), Width = 150, Maximum = 10000000, Increment = 1000, Value = 25000 };
            Label l6 = new Label { Text = "Jumlah Terjual:", Location = new Point(25, 225), AutoSize = true };
            numTerjual = new NumericUpDown { Location = new Point(150, 222), Width = 100, Maximum = 100000, Value = 0 };

            Button btnSimpan = UIHelper.BuatTombolUtama("Simpan", new Point(150, 265), new Size(100, 30), AnchorStyles.Top | AnchorStyles.Left);
            Button btnBatal = UIHelper.BuatTombolSekunder("Batal", new Point(280, 265), new Size(100, 30), AnchorStyles.Top | AnchorStyles.Left);

            if (editData == null)
                txtId.Text = (DataRepository.ListMenu.Count > 0 ? DataRepository.ListMenu.Max(x => x.IdMenu) + 1 : 1).ToString();
            else
            {
                txtId.Text = editData.IdMenu.ToString();
                txtNama.Text = editData.NamaMenu;
                cmbKategori.SelectedItem = editData.KategoriPaket;
                numHarga.Value = editData.Harga;
                numTerjual.Value = editData.Terjual;
                for (int i = 0; i < cmbResto.Items.Count; i++)
                    if (cmbResto.Items[i].ToString().StartsWith(editData.IdRestoran + " -")) cmbResto.SelectedIndex = i;
            }

            btnSimpan.Click += (s, e) =>
            {
                if (cmbResto.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtNama.Text)) { MessageBox.Show("Lengkapi Restoran dan Nama Menu!"); return; }
                int idResto = int.Parse(cmbResto.SelectedItem.ToString().Split('-')[0].Trim());
                if (menuEdit == null)
                    DataRepository.ListMenu.Add(new MenuResto { IdMenu = int.Parse(txtId.Text), IdRestoran = idResto, NamaMenu = txtNama.Text.Trim(), KategoriPaket = cmbKategori.SelectedItem.ToString(), Harga = (int)numHarga.Value, Terjual = (int)numTerjual.Value });
                else
                {
                    menuEdit.IdRestoran = idResto;
                    menuEdit.NamaMenu = txtNama.Text.Trim();
                    menuEdit.KategoriPaket = cmbKategori.SelectedItem.ToString();
                    menuEdit.Harga = (int)numHarga.Value;
                    menuEdit.Terjual = (int)numTerjual.Value;
                }
                this.DialogResult = DialogResult.OK;
            };
            btnBatal.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { l1, txtId, l2, cmbResto, l3, txtNama, l4, cmbKategori, l5, numHarga, l6, numTerjual, btnSimpan, btnBatal });
        }
    }
}

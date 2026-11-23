using System;
using System.Drawing;
using System.Windows.Forms;
using ReservasiLibrary;

namespace RestoranFormsApp
{
    public partial class Form1 : Form
    {
        private bool isAdminLoggedIn = false;
        private Label lblStatusLogin;
        private TextBox txtUser, txtPass;
        private Button btnLogin, btnLogout;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public Form1()
        {
            InitializeComponent();
            DataRepository.MuatSemuaData();
            BangunDashboardUI();
        }

        private void BangunDashboardUI()
        {
            this.Text = "Dashboard Utama - Sistem Informasi Reservasi Restoran";
            this.Size = new Size(780, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 246, 248);
            this.Controls.Clear();

            Label lblJudul = new Label
            {
                Text = "SISTEM INFORMASI RESERVASI RESTORAN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 115),
                AutoSize = true,
                Location = new Point(165, 18)
            };
            this.Controls.Add(lblJudul);

            GroupBox grpLogin = new GroupBox
            {
                Text = "Cek Akses (Admin / Pelanggan",
                Location = new Point(30, 62),
                Size = new Size(705, 95),
                Font = new Font("Segoe UI", 9.5f)
            };

            Label l1 = new Label { Text = "Username:", Location = new Point(20, 30), AutoSize = true };
            txtUser = new TextBox { Location = new Point(95, 27), Width = 120, Text = "admin" };

            Label l2 = new Label { Text = "Password:", Location = new Point(235, 30), AutoSize = true };
            txtPass = new TextBox { Location = new Point(305, 27), Width = 120, PasswordChar = '*', Text = "123" };

            btnLogin = UIHelper.BuatTombolUtama("Login Admin", new Point(445, 25), new Size(110, 28), AnchorStyles.Top | AnchorStyles.Left);
            btnLogout = UIHelper.BuatTombolSekunder("Mode User", new Point(565, 25), new Size(110, 28), AnchorStyles.Top | AnchorStyles.Left);
            btnLogout.Enabled = false;

            lblStatusLogin = new Label
            {
                Text = "Status Akses Saat Ini: PELANGGAN / USER (Tanpa Login)",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(20, 65),
                AutoSize = true
            };

            btnLogin.Click += (s, e) =>
            {
                if (txtUser.Text == "admin" && txtPass.Text == "123")
                {
                    isAdminLoggedIn = true;
                    lblStatusLogin.Text = "Status Akses Saat Ini: ADMIN BACKEND (Full Akses)";
                    lblStatusLogin.ForeColor = Color.DarkGreen;
                    btnLogin.Enabled = false;
                    btnLogout.Enabled = true;
                    MessageBox.Show("Login Admin Berhasil!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Username atau Password Admin salah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnLogout.Click += (s, e) =>
            {
                isAdminLoggedIn = false;
                lblStatusLogin.Text = "Status Akses Saat Ini: PELANGGAN / USER";
                lblStatusLogin.ForeColor = Color.DarkBlue;
                btnLogin.Enabled = true;
                btnLogout.Enabled = false;
            };

            grpLogin.Controls.AddRange(new Control[] { l1, txtUser, l2, txtPass, btnLogin, btnLogout, lblStatusLogin });
            this.Controls.Add(grpLogin);

            GroupBox grpMenu = new GroupBox
            {
                Text = "Modul Manajemen Data & Transaksi",
                Location = new Point(30, 170),
                Size = new Size(705, 290),
                Font = new Font("Segoe UI", 9.5f)
            };

            Button btnResto = UIHelper.BuatTombolUtama("1. Data Restoran", new Point(30, 35), new Size(305, 65), AnchorStyles.Top | AnchorStyles.Left);
            btnResto.Click += (s, e) => new FormRestoranList().ShowDialog();

            Button btnMenu = UIHelper.BuatTombolUtama("2. Katalog Menu & Best Seller", new Point(365, 35), new Size(305, 65), AnchorStyles.Top | AnchorStyles.Left);
            btnMenu.Click += (s, e) => new FormMenuList().ShowDialog();

            Button btnPelanggan = UIHelper.BuatTombolUtama("3. Data Pelanggan", new Point(30, 115), new Size(305, 65), AnchorStyles.Top | AnchorStyles.Left);
            btnPelanggan.Click += (s, e) => new FormPelangganList().ShowDialog();

            Button btnBooking = UIHelper.BuatTombolUtama("4. Antrean Reservasi / Booking)", new Point(365, 115), new Size(305, 65), AnchorStyles.Top | AnchorStyles.Left);
            btnBooking.Click += (s, e) => new FormBookingList(isAdminLoggedIn).ShowDialog();

            Button btnCancel = UIHelper.BuatTombolSekunder("5. Riwayat Pembatalan Reservasi)", new Point(30, 198), new Size(640, 58), AnchorStyles.Top | AnchorStyles.Left);
            btnCancel.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnCancel.Click += (s, e) => new FormPembatalanList().ShowDialog();

            grpMenu.Controls.AddRange(new Control[] { btnResto, btnMenu, btnPelanggan, btnBooking, btnCancel });
            this.Controls.Add(grpMenu);
        }
    }
}

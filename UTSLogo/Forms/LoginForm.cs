using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using UTSLogo.Classes;

namespace UTSLogo.Forms
{
    public partial class LoginForm : XtraForm
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        public LoginForm()
        {
            InitializeComponent();
            SetupFormStyle();
        }
        private void SetupFormStyle()
        {
            this.Region = CreateRoundedRegion(this.Width, this.Height, 15);  
        }
        private void PicUserIcon_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(Color.FromArgb(230, 126, 34)))
            using (var font = new Font("Segoe UI", 24F, FontStyle.Bold))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                
            }
        }
        private Region CreateRoundedRegion(int width, int height, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(width - radius * 2, height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseAllFigures();
            return new Region(path);
        }
        private Region CreateCircleRegion(int diameter)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, diameter, diameter);
            return new Region(path);
        }
        private void pnl_Header_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void btn_Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            txt_Password.Properties.PasswordChar = '*';
            SetupHoverEffects();
            txt_UserName.Focus();
        }
        private void SetupHoverEffects()
        {
            btn_Login.MouseEnter += (s, e) => {
                btn_Login.Appearance.BackColor = Color.FromArgb(211, 84, 0); 
            };
            btn_Login.MouseLeave += (s, e) => {
                btn_Login.Appearance.BackColor = Color.FromArgb(230, 126, 34); 
            };
            btn_Close.MouseEnter += (s, e) => {
                btn_Close.Appearance.BackColor = Color.FromArgb(192, 57, 43); 
            };
            btn_Close.MouseLeave += (s, e) => {
                btn_Close.Appearance.BackColor = Color.Transparent;
            };
        }
        private void btn_Eyes_Click(object sender, EventArgs e)
        {
            txt_Password.Properties.PasswordChar = '\0';
            btn_Eyes.Visible = false;
            btn_NotEye.Visible = true;
            txt_Password.Focus();
        }
        private void btn_NotEye_Click(object sender, EventArgs e)
        {
            txt_Password.Properties.PasswordChar = '*';
            btn_Eyes.Visible = true;
            btn_NotEye.Visible = false;
            txt_Password.Focus();
        }
        private async void btn_Login_Click(object sender, EventArgs e)
        {
            string username = txt_UserName.Text.Trim();
            string password = txt_Password.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                XtraMessageBox.Show("Kullanıcı adı ve şifre giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            btn_Login.Enabled = false;
            btn_Login.Text = "  ⏳  Giriş yapılıyor...";
            try
            {
                DataTable sqlCheck = await SQLiteCrud.GetDataFromSQLiteAsync("SELECT * FROM SQLConnectionString LIMIT 1");
                if (!DataHelper.IsDataExists(sqlCheck))
                {
                    using (SQLConnectionSettingsForm sqlForm = new SQLConnectionSettingsForm())
                    {
                        sqlForm.ShowDialog();
                        if (!sqlForm.DialogResult.Equals(DialogResult.OK))
                        {
                            XtraMessageBox.Show("SQL bağlantısı yapılmadan giriş yapılamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                string query = "SELECT * FROM Users WHERE UserName=@user";
                Dictionary<string, object> param = new Dictionary<string, object>() { { "@user", username } };
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query, param);
                if (!DataHelper.IsDataExists(dt))
                {
                    XtraMessageBox.Show("Kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DataRow row = dt.Rows[0];
                string decryptedPassword = await EncryptionHelper.Decrypt(row["Password"].ToString());
                if (password != decryptedPassword)
                {
                    XtraMessageBox.Show("Şifre yanlış.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int status = Convert.ToInt32(row["Status"]);
                if (status == 1)
                {
                    XtraMessageBox.Show("Kullanıcı pasif, giriş yapılamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string firmaNr = row["FirmaNR"]?.ToString();
                string periodNo = row["PeriodNR"]?.ToString();
                if (string.IsNullOrEmpty(firmaNr) || string.IsNullOrEmpty(periodNo))
                {
                    using (FormCompanyChoose frmCompany = new FormCompanyChoose())
                    {
                        frmCompany.ShowDialog();
                        if (string.IsNullOrEmpty(frmCompany.companyNr))
                        {
                            XtraMessageBox.Show("Firma seçimi yapılmadı, giriş iptal edildi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        firmaNr = frmCompany.companyNr;
                        periodNo = frmCompany.periodNo;
                        string updateQuery = "UPDATE Users SET FirmaNR=@firma, PeriodNR=@period WHERE UserName=@user";
                        Dictionary<string, object> updateParam = new Dictionary<string, object>()
                        {
                            { "@firma", firmaNr },
                            { "@period", periodNo },
                            { "@user", username }
                        };
                        await SQLiteCrud.InsertUpdateDeleteAsync(updateQuery, updateParam);
                    }
                }
                HomeForm home = new HomeForm
                {
                    userName = username,
                    firmaNr = firmaNr,
                    periodNo = periodNo
                };
                home.Show();
                this.Hide();
            }
            finally
            {
                btn_Login.Enabled = true;
                btn_Login.Text = "  🚀  Giriş Yap";
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "https://mutluyazilim.com.tr/",
                    UseShellExecute = true 
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Tarayıcı açılamadı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void lbl_Title_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
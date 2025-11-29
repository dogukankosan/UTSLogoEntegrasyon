using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using UTSLogo.Classes;

namespace UTSLogo.Forms
{
    public partial class LoginForm : XtraForm
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            txt_UserName.Focus();
            txt_Password.Properties.PasswordChar = '*';
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
            var param = new Dictionary<string, object>() { { "@user", username } };
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
                    var updateParam = new Dictionary<string, object>()
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
    }
}
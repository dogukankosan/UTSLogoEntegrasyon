using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using UTSLogo.Classes;

namespace UTSLogo.Forms
{
    public partial class HomeForm : XtraForm
    {
        public HomeForm()
        {
            InitializeComponent();
            // Tema değişimini dinle
            UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;
        }

        public string userName = "";
        public string firmaNr = "";
        public string periodNo = "";
        public bool isAdmin = false; // Admin yetkisi

        internal void OpenFormInContainer(Form form)
        {
            if (form == null) return;
            try
            {
                for (int i = panelControl1.Controls.Count - 1; i >= 0; i--)
                {
                    if (!(panelControl1.Controls[i] is PictureBox))
                        panelControl1.Controls.RemoveAt(i);
                }
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                panelControl1.Controls.Add(form);
                form.BringToFront();
                form.Show();
            }
            catch (Exception) { }
        }

        private async void HomeForm_Load(object sender, EventArgs e)
        {
            UpdateUserInfoDisplay();

            // DevExpress bonus skinleri ve form skinlerini aktifleştir
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Skins.SkinManager.EnableFormSkins();

            // Kullanıcının kaydettiği temayı uygula
            await LoadUserThemeAsync();

            // Kullanıcı admin değilse accordionControlElement1 gizle
            accordionControlElement1.Visible = isAdmin;
        }

        private void UpdateUserInfoDisplay()
        {
            if (!string.IsNullOrEmpty(userName))
            {
                string htmlText =
                    $"<u><b>Kullanıcı Adı:</b></u> {userName}<br>" +
                    $"<u><b>Şirket Kodu:</b></u> {firmaNr.PadLeft(3, '0')}<br>" +
                    $"<u><b>Dönem No:</b></u> {periodNo.PadLeft(2, '0')}";
                companyChooseControlElement2.Text = htmlText;
                companyChooseControlElement2.Appearance.Normal.Options.UseTextOptions = true;
                companyChooseControlElement2.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            }
        }

        private async Task LoadUserThemeAsync()
        {
            try
            {
                string query = "SELECT Thema, IsAdmin FROM Users WHERE UserName=@user LIMIT 1";
                var param = new Dictionary<string, object> { { "@user", userName } };
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string savedTheme = dt.Rows[0]["Thema"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(savedTheme))
                        UserLookAndFeel.Default.SkinName = savedTheme;
                    else
                        UserLookAndFeel.Default.SkinName = "Basic";

                    // Admin kontrolü
                    isAdmin = Convert.ToInt32(dt.Rows[0]["IsAdmin"]) == 1;
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(userName, "Tema yükleme hatası: " + ex.ToString());
            }
        }

        // Kullanıcı tema değiştirdiğinde tetiklenir
        private async void Default_StyleChanged(object sender, EventArgs e)
        {
            try
            {
                string currentTheme = UserLookAndFeel.Default.ActiveSkinName;
                var updateParams = new Dictionary<string, object>
                {
                    { "@Thema", currentTheme },
                    { "@UserName", userName }
                };
                string updateSql = "UPDATE Users SET Thema = @Thema WHERE UserName = @UserName COLLATE NOCASE";
                await SQLiteCrud.InsertUpdateDeleteAsync(updateSql, updateParams);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Tema kaydetme hatası:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(userName, "Tema kaydetme hatası: " + ex.ToString());
            }
        }

        // Popup menü ile tema seçimi
        private void themaControlElement3_Click(object sender, EventArgs e)
        {
            popupMenu2.ShowPopup(Cursor.Position);
        }

        private async void companyChooseControlElement2_Click(object sender, EventArgs e)
        {
            FormCompanyChoose fr = new FormCompanyChoose();
            fr.ShowDialog();

            if (string.IsNullOrEmpty(fr.companyNr) || string.IsNullOrEmpty(fr.companyName) || string.IsNullOrEmpty(fr.periodNo))
                return;

            firmaNr = fr.companyNr;
            periodNo = fr.periodNo;

            // SQL’de de güncelle
            var updateParams = new Dictionary<string, object>
            {
                { "@FirmaNR", firmaNr },
                { "@PeriodNR", periodNo },
                { "@UserName", userName }
            };
            string updateSql = "UPDATE Users SET FirmaNR=@FirmaNR, PeriodNR=@PeriodNR WHERE UserName=@UserName COLLATE NOCASE";
            await SQLiteCrud.InsertUpdateDeleteAsync(updateSql, updateParams);

            UpdateUserInfoDisplay();
        }

        // Panel form açma
        private void sqlSettingsControlElement1_Click(object sender, EventArgs e) => OpenFormInContainer(new SQLConnectionSettingsForm());
        private void invoicesControlElement2_Click(object sender, EventArgs e) { }
        private void salesInvoicesControlElement2_Click(object sender, EventArgs e) { OpenFormInContainer(new SalesInvoicesForm(userName,firmaNr.PadLeft(3, '0'),periodNo.PadLeft(2, '0'))); }
        private void usersControlElement2_Click_1(object sender, EventArgs e) => OpenFormInContainer(new UsersForm());
        private void UTSControlElement3_Click(object sender, EventArgs e) => OpenFormInContainer(new UTSForm());
        private void allListErrorControlElement3_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new ErrorListForm());
        }
        private void HomeForm_FormClosing(object sender, FormClosingEventArgs e) => Application.Exit();
        private void UsererrorListControlElement3_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new ErrorListForm(userName));
        }
    }
}

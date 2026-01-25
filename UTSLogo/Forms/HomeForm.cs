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
            UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;
        }
        public string userName = "";
        public string firmaNr = "";
        public string periodNo = "";
        public bool isAdmin = false;
        // ✅ YENİ ALANLAR
        private bool _isLot = false;
        private bool _isEirs = false;
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
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            await LoadUserThemeAsync();
            // ✅ YENİ: IsLot ve IsEirs ayarlarını yükle
            await LoadClientSettingsAsync();
            // ✅ YENİ: Menü görünürlüklerini ayarla
            ConfigureMenuVisibility();
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
                Dictionary<string, object> param = new Dictionary<string, object> { { "@user", userName } };
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string savedTheme = dt.Rows[0]["Thema"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(savedTheme))
                        UserLookAndFeel.Default.SkinName = savedTheme;
                    else
                        UserLookAndFeel.Default.SkinName = "Basic";
                    isAdmin = Convert.ToInt32(dt.Rows[0]["IsAdmin"]) == 1;
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(userName, "Tema yükleme hatası: " + ex.ToString());
            }
        }

        #region ==================== CLIENT SETTINGS YÜKLEME ====================
        private async Task LoadClientSettingsAsync()
        {
            try
            {
                string query = "SELECT IsLot, IsEirs FROM ClientSettings LIMIT 1";
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query);
                if (dt?.Rows.Count > 0)
                {
                    _isLot = Convert.ToInt32(dt.Rows[0]["IsLot"]) == 1;
                    _isEirs = Convert.ToInt32(dt.Rows[0]["IsEirs"]) == 1;
                }
                else
                {
                    _isLot = false;
                    _isEirs = false;
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(userName, $"ClientSettings yükleme hatası: {ex.Message}");
                _isLot = false;
                _isEirs = false;
            }
        }
        #endregion

        #region ==================== PUBLIC MENÜ YENİLEME ====================
        // ✅ YENİ: UTSForm'dan çağrılabilir public metod
        public async Task RefreshClientSettings()
        {
            await LoadClientSettingsAsync();
            ConfigureMenuVisibility();
        }
        #endregion

        #region ==================== MENÜ GÖRÜNÜRLÜĞÜ AYARLAMA ====================
        private void ConfigureMenuVisibility()
        {
            if (_isLot && _isEirs)
            {
                // SENARYO 1: Sadece İrsaliye
                salesInvoicesControlElement2.Visible = false;
                salesTruckControlElement3.Visible = true;
            }
            else if (_isLot && !_isEirs)
            {
                // SENARYO 2: Sadece Fatura
                salesInvoicesControlElement2.Visible = true;
                salesTruckControlElement3.Visible = false;
            }
            else if (!_isLot && !_isEirs)
            {
                // SENARYO 3: Sadece Fatura
                salesInvoicesControlElement2.Visible = true;
                salesTruckControlElement3.Visible = false;
            }
            else if (!_isLot && _isEirs)
            {
                // SENARYO 4: Hem Fatura hem İrsaliye
                salesInvoicesControlElement2.Visible = true;
                salesTruckControlElement3.Visible = true;
            }
        }
        #endregion

        private async void Default_StyleChanged(object sender, EventArgs e)
        {
            try
            {
                string currentTheme = UserLookAndFeel.Default.ActiveSkinName;
                Dictionary<string, object> updateParams = new Dictionary<string, object>
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
            Dictionary<string, object> updateParams = new Dictionary<string, object>
            {
                { "@FirmaNR", firmaNr },
                { "@PeriodNR", periodNo },
                { "@UserName", userName }
            };
            string updateSql = "UPDATE Users SET FirmaNR=@FirmaNR, PeriodNR=@PeriodNR WHERE UserName=@UserName COLLATE NOCASE";
            await SQLiteCrud.InsertUpdateDeleteAsync(updateSql, updateParams);
            UpdateUserInfoDisplay();
        }
        private void sqlSettingsControlElement1_Click(object sender, EventArgs e) => OpenFormInContainer(new SQLConnectionSettingsForm());
        private void salesInvoicesControlElement2_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new SalesInvoicesForm(userName, firmaNr.PadLeft(3, '0'), periodNo.PadLeft(2, '0')));
        }
        private void usersControlElement2_Click_1(object sender, EventArgs e) => OpenFormInContainer(new UsersForm());
        private void UTSControlElement3_Click(object sender, EventArgs e) => OpenFormInContainer(new UTSForm(this));
        private void allListErrorControlElement3_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new ErrorListForm());
        }
        private void HomeForm_FormClosing(object sender, FormClosingEventArgs e) => Application.Exit();
        private void UsererrorListControlElement3_Click(object sender, EventArgs e)
        {
            OpenFormInContainer(new ErrorListForm(userName));
        }
        private void salesTruckControlElement3_Click(object sender, EventArgs e)
        {
            // ✅ DÜZELTİLDİ: Parametreler eklendi
            OpenFormInContainer(new SalesTruckForm(userName, firmaNr.PadLeft(3, '0'), periodNo.PadLeft(2, '0')));
        }
    }
}
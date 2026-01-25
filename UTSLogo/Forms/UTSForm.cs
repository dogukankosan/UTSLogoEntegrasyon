using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using UTSLogo.Classes;

namespace UTSLogo.Forms
{
    public partial class UTSForm : XtraForm
    {
        private string _customerGUID;
        private string _customerToken;
        private bool _isRegistered;
        private HomeForm _homeForm;
        public UTSForm(HomeForm homeForm = null)
        {
            InitializeComponent();
            _homeForm = homeForm;
        }
        private async void UTSForm_Load(object sender, EventArgs e) => await LoadSettingsAsync();

        #region ==================== AYARLAR YÜKLEME ====================
        private async Task LoadSettingsAsync()
        {
            try
            {
                string query = "SELECT CustomerGUID, CustomerToken, IsLot, IsEirs FROM ClientSettings LIMIT 1";
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query);
                if (dt?.Rows.Count > 0)
                {
                    _isRegistered = true;
                    _customerGUID = dt.Rows[0]["CustomerGUID"].ToString();
                    string encryptedToken = dt.Rows[0]["CustomerToken"].ToString();
                    _customerToken = await EncryptionHelper.Decrypt(encryptedToken);
                    chk_ISLot.Checked = Convert.ToInt32(dt.Rows[0]["IsLot"]) == 1;
                    chk_Eirs.Checked = Convert.ToInt32(dt.Rows[0]["IsEirs"]) == 1; // ✅ YENİ
                    SetFormLocked(true);
                    await RefreshFromAPIAsync();
                }
                else
                {
                    _isRegistered = false;
                    SetFormLocked(false);
                    XtraMessageBox.Show(
                        "Henüz kayıtlı müşteri bilgisi yok.\nLütfen bilgileri doldurup Kaydet butonuna basın.",
                        "İlk Kurulum", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("System", $"LoadSettingsAsync hata: {ex.Message}");
                XtraMessageBox.Show("Ayarlar yüklenirken hata oluştu:\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            txt_CustomerName.Focus();
        }
        private void SetFormLocked(bool locked)
        {
            txt_CustomerName.Properties.ReadOnly = locked;
            txt_CustomerToken.Properties.ReadOnly = locked;
            nmr_Count.ReadOnly = locked;
            txt_Token.Properties.ReadOnly = false;
        }
        #endregion

        #region ==================== API İŞLEMLERİ ====================
        private async Task RefreshFromAPIAsync()
        {
            if (!_isRegistered || string.IsNullOrEmpty(_customerGUID) || string.IsNullOrEmpty(_customerToken))
                return;
            Cursor = Cursors.WaitCursor;
            try
            {
                var result = await WebAPIClient.GetCustomerInfoAsync(_customerGUID, _customerToken, "UI");
                if (result.success)
                {
                    txt_CustomerName.Text = result.customerName;
                    nmr_Count.Value = result.count;
                    txt_Token.Text = result.utsToken;
                    txt_CustomerToken.Text = "••••••••••••••••";
                }
                else
                {
                    await TextLog.LogToSQLiteAsync("UI", $"RefreshFromAPIAsync: {result.message}");
                    XtraMessageBox.Show("API'den bilgi alınamadı:\n" + result.message,
                        "API Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally { Cursor = Cursors.Default; }
        }
        #endregion

        #region ==================== KAYDET BUTONU ====================
        private async void btn_Save_Click(object sender, EventArgs e)
        {
            btn_Save.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                if (_isRegistered)
                    await UpdateUTSTokenAsync();
                else
                    await RegisterNewCustomerAsync();
                // ✅ YENİ: Kayıt sonrası menüleri güncelle
                await RefreshHomeMenusAsync();
            }
            finally
            {
                Cursor = Cursors.Default;
                btn_Save.Enabled = true;
            }
        }
        private async Task RegisterNewCustomerAsync()
        {
            string customerName = txt_CustomerName.Text.Trim();
            string customerToken = txt_CustomerToken.Text.Trim();
            string utsToken = txt_Token.Text.Trim();
            int count = (int)nmr_Count.Value;
            if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerToken)
                || string.IsNullOrWhiteSpace(utsToken) || count < 0)
            {
                XtraMessageBox.Show("Lütfen tüm alanları doğru doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var apiResult = await WebAPIClient.RegisterCustomerAsync(customerName, utsToken, count, customerToken);
            if (!apiResult.success)
            {
                await TextLog.LogToSQLiteAsync("UI", $"RegisterNewCustomerAsync API: {apiResult.message}");
                XtraMessageBox.Show("Kayıt başarısız:\n" + apiResult.message, "API Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string encryptedToken = await EncryptionHelper.Encrypt(customerToken);
            // ✅ IsEirs parametresi eklendi
            string insertQuery = "INSERT INTO ClientSettings (CustomerGUID, CustomerToken, IsLot, IsEirs) VALUES (@guid, @token, @isLot, @isEirs)";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@guid", apiResult.customerGUID },
                { "@token", encryptedToken },
                { "@isLot", chk_ISLot.Checked ? 1 : 0 },
                { "@isEirs", chk_Eirs.Checked ? 1 : 0 } // ✅ YENİ
            };
            var sqliteResult = await SQLiteCrud.InsertUpdateDeleteAsync(insertQuery, parameters);
            if (!sqliteResult.Success)
            {
                await TextLog.LogToSQLiteAsync("UI", $"RegisterNewCustomerAsync SQLite: {sqliteResult.ErrorMessage}");
                XtraMessageBox.Show("API'ye kaydedildi fakat yerel DB kaydedilemedi:\n" + sqliteResult.ErrorMessage,
                    "Yerel Kayıt Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            _customerGUID = apiResult.customerGUID;
            _customerToken = customerToken;
            _isRegistered = true;
            SetFormLocked(true);
            txt_CustomerToken.Text = "••••••••••••••••";
            XtraMessageBox.Show("Müşteri başarıyla kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private async Task UpdateUTSTokenAsync()
        {
            string utsToken = txt_Token.Text.Trim();
            if (string.IsNullOrWhiteSpace(utsToken))
            {
                XtraMessageBox.Show("UTS Token boş olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var result = await WebAPIClient.UpdateUTSTokenAsync(_customerGUID, utsToken, _customerToken, "UI");
            if (!result.success)
            {
                await TextLog.LogToSQLiteAsync("UI", $"UpdateUTSTokenAsync: {result.message}");
                XtraMessageBox.Show("UTS Token güncellenemedi:\n" + result.message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                XtraMessageBox.Show("UTS Token başarıyla güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region ==================== IsLot GÜNCELLEME ====================
        private async Task UpdateIsLotAsync()
        {
            try
            {
                string updateQuery = "UPDATE ClientSettings SET IsLot = @isLot WHERE CustomerGUID = @guid";
                Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@isLot", chk_ISLot.Checked ? 1 : 0 },
                { "@guid", _customerGUID }
            };
                var result = await SQLiteCrud.InsertUpdateDeleteAsync(updateQuery, parameters);
                if (!result.Success)
                    await TextLog.LogToSQLiteAsync("UI", $"UpdateIsLotAsync: {result.ErrorMessage}");
                // ✅ YENİ: Güncelleme sonrası menüleri yenile
                await RefreshHomeMenusAsync();
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("UI", $"UpdateIsLotAsync hata: {ex.Message}");
            }
        }
        #endregion

        #region ==================== IsEirs GÜNCELLEME ====================
        private async Task UpdateIsEirsAsync()
        {
            try
            {
                string updateQuery = "UPDATE ClientSettings SET IsEirs = @isEirs WHERE CustomerGUID = @guid";
                Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@isEirs", chk_Eirs.Checked ? 1 : 0 },
                { "@guid", _customerGUID }
            };
                var result = await SQLiteCrud.InsertUpdateDeleteAsync(updateQuery, parameters);
                if (!result.Success)
                    await TextLog.LogToSQLiteAsync("UI", $"UpdateIsEirsAsync: {result.ErrorMessage}");
                // ✅ YENİ: Güncelleme sonrası menüleri yenile
                await RefreshHomeMenusAsync();
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("UI", $"UpdateIsEirsAsync hata: {ex.Message}");
            }
        }
        #endregion

        #region ==================== YENİLE BUTONU ====================
        private async void btn_Refresh_Click(object sender, EventArgs e)
        {
            if (!_isRegistered)
            {
                XtraMessageBox.Show("Önce müşteri kaydı yapmalısınız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            await RefreshFromAPIAsync();
        }
        #endregion

        private async void chk_ISLot_CheckedChanged_1(object sender, EventArgs e)
        {
            if (!_isRegistered) return;
            await UpdateIsLotAsync();
        }
        // ✅ YENİ EVENT HANDLER
        private async void chk_Eirs_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isRegistered) return;
            await UpdateIsEirsAsync();
        }
        #region ==================== HOME MENÜ YENİLEME ====================
        // ✅ YENİ METOD
        private async Task RefreshHomeMenusAsync()
        {
            if (_homeForm != null)
                await _homeForm.RefreshClientSettings();
        }
        #endregion
    }
}
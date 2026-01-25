using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using UTSLogo.Classes;

namespace UTSLogo.Forms
{
    public partial class SalesTruckDetailsForm : XtraForm
    {
        private string _logicalRef;
        private string _userName;
        private string _firmaNr;
        private string _periodNo;

        public SalesTruckDetailsForm(string logicalRef, string userName, string firmaNr, string periodNo)
        {
            InitializeComponent();
            _logicalRef = logicalRef;
            _userName = userName;
            _firmaNr = firmaNr;
            _periodNo = periodNo;
        }
        private async void SalesTruckDetailsForm_Load(object sender, EventArgs e)
        {
            await LoadTruckDetailsAsync();
        }
        private async Task LoadTruckDetailsAsync()
        {
            try
            {
                string firma = _firmaNr.PadLeft(3, '0');
                string period = _periodNo.PadLeft(2, '0');
                string query = $@"
                    SELECT 
                        SL.LOGICALREF,
                        IT.CODE AS [Malzeme Kodu],
                        IT.NAME AS [Malzeme Adı],
                        IT.GTIN_UNO AS [UNO],
                        SL.AMOUNT AS [Miktar],
                        UN.CODE AS [Birim],
                        SL.PRICE AS [Birim Fiyat],
                        SL.TOTAL AS [Satır Toplamı],
                        SL.VAT AS [KDV Oranı],
                        SL.VATAMNT AS [KDV Tutarı],
                        SL.LINENET AS [Net Tutar],
                        SL.LINEEXP AS [Satır Açıklama],
                        CASE 
                            WHEN SL.LINETYPE = 0 THEN 'Normal'
                            WHEN SL.LINETYPE = 1 THEN 'Promosyon'
                            WHEN SL.LINETYPE = 2 THEN 'Bedava'
                            WHEN SL.LINETYPE = 3 THEN 'Masraf'
                            WHEN SL.LINETYPE = 4 THEN 'İndirim'
                            WHEN SL.LINETYPE = 8 THEN 'Paket'
                            ELSE 'Diğer'
                        END AS [Satır Tipi],
                        CASE 
                            WHEN SL.CANCELLED = 0 THEN 'Aktif'
                            ELSE 'İptal'
                        END AS [Durum]
                    FROM 
                        LG_{firma}_{period}_STLINE SL WITH(NOLOCK)
                    LEFT JOIN 
                        LG_{firma}_ITEMS IT WITH(NOLOCK) ON IT.LOGICALREF = SL.STOCKREF
                    LEFT JOIN 
                        LG_{firma}_UNITSETL UN WITH(NOLOCK) ON UN.LOGICALREF = SL.UOMREF
                    WHERE 
                        SL.STFICHEREF = @LogicalRef
                        AND SL.LINETYPE IN (0, 1, 2, 3, 4, 8)
                    ORDER BY 
                        SL.LOGICALREF";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@LogicalRef", _logicalRef }
                };
                DataTable dt = await SQLCrud.GetDataTableAsync(query, parameters);
                if (dt == null || dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Bu irsaliyeye ait satır bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // ✅ YENİ: İrsaliye numarasını al
                string irsaliyeNo = await GetIrsaliyeNoAsync(firma, period);
                int satirSayisi = dt.Rows.Count;
                // ✅ YENİ: Form başlığını güncelle
                this.Text = $"İrsaliye Detayları (LogicalRef : {_logicalRef}) - Satır Sayısı: {satirSayisi}";
                // UTS durumunu ekle
                dt.Columns.Add("UTS Durumu", typeof(string));
                // SQLite'dan UTS kayıtlarını çek
                DataTable utsKayitlar = await GetUtsKayitlariAsync();
                HashSet<string> cekilmisSatirlar = new HashSet<string>();
                if (utsKayitlar != null && utsKayitlar.Rows.Count > 0)
                {
                    foreach (DataRow row in utsKayitlar.Rows)
                    {
                        string lineRef = row["STLINE_LOGICALREF"]?.ToString();
                        if (!string.IsNullOrEmpty(lineRef))
                            cekilmisSatirlar.Add(lineRef);
                    }
                }
                // Her satır için UTS durumunu güncelle
                foreach (DataRow row in dt.Rows)
                {
                    string logicalRef = row["LOGICALREF"]?.ToString();
                    if (cekilmisSatirlar.Contains(logicalRef))
                        row["UTS Durumu"] = "✓ Çekildi";
                    else
                        row["UTS Durumu"] = "Çekilmedi";
                }
                gridControl1.DataSource = dt;
                if (gridView1.Columns["LOGICALREF"] != null)
                    gridView1.Columns["LOGICALREF"].Visible = false;
                if (gridView1.Columns["UTS Durumu"] != null)
                    gridView1.Columns["UTS Durumu"].VisibleIndex = 0;
                gridView1.BestFitColumns();
                GridViewDesigner.CustomizeGrid(gridView1);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"İrsaliye detayları yüklenemedi:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(_userName, $"İrsaliye Detay Yükleme Hatası: {ex.Message}");
            }
        }

        #region ==================== İRSALİYE NUMARASI ALMA ====================
        // ✅ YENİ METOD
        private async Task<string> GetIrsaliyeNoAsync(string firma, string period)
        {
            try
            {
                string query = $@"
                    SELECT FICHENO 
                    FROM LG_{firma}_{period}_STFICHE WITH(NOLOCK)
                    WHERE LOGICALREF = @LogicalRef";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@LogicalRef", _logicalRef }
                };
                DataTable dt = await SQLCrud.GetDataTableAsync(query, parameters);
                if (dt != null && dt.Rows.Count > 0)
                    return dt.Rows[0]["FICHENO"]?.ToString() ?? _logicalRef;
                return _logicalRef;
            }
            catch
            {
                return _logicalRef;
            }
        }
        #endregion

        private async Task<DataTable> GetUtsKayitlariAsync()
        {
            try
            {
                string query = @"
                    SELECT DISTINCT STLINE_LOGICALREF 
                    FROM UTSCekimKayitlari 
                    WHERE UTSdenCekildi = 1 
                    AND STFICHEREF = @StficheRef";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@StficheRef", _logicalRef }
                };
                return await SQLiteCrud.GetDataFromSQLiteAsync(query, parameters);
            }
            catch
            {
                return null;
            }
        }
        #region ==================== PUBLIC YENİLEME METODU ====================
        public async Task RefreshDetailsAsync()
        {
            await LoadTruckDetailsAsync();
        }
        #endregion

        private void SalesTruckDetailsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Escape)
                this.Close();
        }
    }
}
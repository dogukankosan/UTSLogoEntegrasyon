using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using UTSLogo.Classes;
using UTSLogo.Models;

namespace UTSLogo.Forms
{
    public partial class SalesTruckForm : XtraForm
    {
        private string _userName;
        private string _firmaNr;
        private string _periodNo;
        private bool _isLot;
        public SalesTruckForm(string userName, string firmaNr, string periodNo)
        {
            InitializeComponent();
            _userName = userName;
            _firmaNr = firmaNr;
            _periodNo = periodNo;
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
        }
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs args = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(args.Location);
            if (hitInfo.InRow || hitInfo.InRowCell)
            {
                string logicalRef = view.GetFocusedRowCellValue("LOGICALREF")?.ToString();
                if (!string.IsNullOrEmpty(logicalRef))
                {
                    SalesTruckDetailsForm detailForm = new SalesTruckDetailsForm(
                        logicalRef,
                        _userName,
                        _firmaNr,
                        _periodNo
                    );
                    detailForm.ShowDialog();
                }
            }
        }
        private async void SalesTruckForm_Load(object sender, EventArgs e)
        {
            await LoadIsLotSettingAsync();
            await LoadSalesTrucksAsync();
        }

        #region ==================== IsLot AYARI YÜKLEME ====================
        private async Task LoadIsLotSettingAsync()
        {
            try
            {
                string query = "SELECT IsLot FROM ClientSettings LIMIT 1";
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query);
                if (dt?.Rows.Count > 0)
                    _isLot = Convert.ToInt32(dt.Rows[0]["IsLot"]) == 1;
                else
                    _isLot = false;
            }
            catch
            {
                _isLot = false;
            }
        }
        #endregion

        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await LoadSalesTrucksAsync();
        }
        private async void UTSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRow() == null)
            {
                XtraMessageBox.Show("Lütfen bir irsaliye seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string logicalRef = gridView1.GetFocusedRowCellValue("LOGICALREF")?.ToString();
            string irsaliyeNo = gridView1.GetFocusedRowCellValue("İrsaliye No")?.ToString();
            string irsaliyeTarihi = gridView1.GetFocusedRowCellValue("İrsaliye Tarihi")?.ToString();
            if (string.IsNullOrEmpty(logicalRef)) return;
            string utsDurumu = gridView1.GetFocusedRowCellValue("UTS Durumu")?.ToString();
            if (utsDurumu == "✓ Çekildi")
            {
                DialogResult result = XtraMessageBox.Show(
                    $"Bu irsaliye daha önce UTS'den çekilmiş.\n\nTekrar çekmek istiyor musunuz?",
                    "Uyarı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;
            }
            await CheckAndCallUtsForTruckAsync(logicalRef, irsaliyeNo, irsaliyeTarihi);
            await LoadSalesTrucksAsync();
        }
        private async Task LoadSalesTrucksAsync()
        {
            try
            {
                string firma = _firmaNr.PadLeft(3, '0');
                string period = _periodNo.PadLeft(2, '0');
                DataTable utsKayitlar = await GetUtsCekilmiIrsaliyelerAsync();
                HashSet<string> cekilmisIrsaliyeler = new HashSet<string>();
                if (utsKayitlar != null && utsKayitlar.Rows.Count > 0)
                {
                    foreach (DataRow row in utsKayitlar.Rows)
                    {
                        string stficheRef = row["STFICHEREF"]?.ToString();
                        if (!string.IsNullOrEmpty(stficheRef))
                            cekilmisIrsaliyeler.Add(stficheRef);
                    }
                }
                string query = $@"
                    SELECT 
                        STF.LOGICALREF,
                        STF.DATE_ AS [IrsaliyeTarihiRaw],
                        CONVERT(VARCHAR(20), STF.DATE_, 104) AS [İrsaliye Tarihi],
                        STF.FICHENO AS [İrsaliye No],
                        CASE 
                            WHEN STF.TRCODE = 2 THEN 'Perakende Satış İade'
                            WHEN STF.TRCODE = 3 THEN 'Toptan Satış İade'
                            WHEN STF.TRCODE = 4 THEN 'Konsinye Çıkış İade'
                            WHEN STF.TRCODE = 7 THEN 'Perakende Satış'
                            WHEN STF.TRCODE = 8 THEN 'Toptan Satış'
                            WHEN STF.TRCODE = 9 THEN 'Konsinye Çıkış'
                            ELSE 'Diğer'
                        END AS [İrsaliye Tipi],
                        CL.CODE AS [Cari Kodu],
                        CL.DEFINITION_ AS [Cari Adı],
                       CONCAT( STF.GENEXP1,' - ',STF.GENEXP2,' - ',STF.GENEXP3,' - ',STF.GENEXP4,' - ',STF.GENEXP5,' - ',STF.GENEXP6) AS [İrsaliye Açıklama],
                        STF.NETTOTAL AS [TL Tutarı],
                        STF.TRNET AS [Döviz Tutarı],
                        ISNULL(CURR.CURCODE, 'TL') AS [Döviz Türü],
                        CASE 
                            WHEN STF.BILLED = 1 THEN 'Faturalandı'
                            ELSE 'Faturalanmadı'
                        END AS [Fatura Durumu],
                        CASE 
                            WHEN STF.EDESPSTATUS = 0 THEN 'Bekliyor'
                            WHEN STF.EDESPSTATUS = 1 THEN 'Gönderildi'
                            WHEN STF.EDESPSTATUS = 2 THEN 'Kabul Edildi'
                            WHEN STF.EDESPSTATUS = 3 THEN 'Red Edildi'
                            ELSE '-'
                        END AS [E-İrsaliye Durumu],
                        INV.FICHENO AS [Bağlı Fatura No]
                    FROM 
                        LG_{firma}_{period}_STFICHE STF WITH(NOLOCK)
                    LEFT JOIN 
                        LG_{firma}_CLCARD CL WITH(NOLOCK) ON CL.LOGICALREF = STF.CLIENTREF
                    LEFT JOIN 
                        L_CURRENCYLIST CURR WITH(NOLOCK) ON CURR.CURTYPE = STF.TRCURR AND CURR.FIRMNR = {firma}
                    LEFT JOIN 
                        LG_{firma}_{period}_INVOICE INV WITH(NOLOCK) ON INV.LOGICALREF = STF.INVOICEREF
           
         WHERE 
                        STF.TRCODE IN (6,10,7,8,9,35,36,37,38,39)
       AND STF.EDESPSTATUS IN (0,1) 
                        AND STF.CANCELLED = 0
                        AND STF.EDESPATCH IN( 1,2)
                    ORDER BY 
                        STF.DATE_ DESC, STF.FTIME DESC, STF.LOGICALREF DESC";
                DataTable dt = await SQLCrud.GetDataTableAsync(query, null);
                dt.Columns.Add("UTS Durumu", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    string logicalRef = row["LOGICALREF"]?.ToString();
                    if (cekilmisIrsaliyeler.Contains(logicalRef))
                        row["UTS Durumu"] = "✓ Çekildi";
                    else
                        row["UTS Durumu"] = "Çekilmedi";
                }
                gridControl1.DataSource = dt;
                if (gridView1.Columns["LOGICALREF"] != null)
                    gridView1.Columns["LOGICALREF"].Visible = false;
                if (gridView1.Columns["IrsaliyeTarihiRaw"] != null)
                    gridView1.Columns["IrsaliyeTarihiRaw"].Visible = false;
                if (gridView1.Columns["UTS Durumu"] != null)
                    gridView1.Columns["UTS Durumu"].VisibleIndex = 0;
                gridView1.BestFitColumns();
                GridViewDesigner.CustomizeGrid(gridView1);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"İrsaliyeler yüklenemedi:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(_userName, $"İrsaliye Listeleme Hatası: {ex.Message}");
            }
        }
        private async Task<DataTable> GetUtsCekilmiIrsaliyelerAsync()
        {
            try
            {
                string query = "SELECT DISTINCT STFICHEREF FROM UTSCekimKayitlari WHERE UTSdenCekildi = 1 AND STFICHEREF IS NOT NULL";
                return await SQLiteCrud.GetDataFromSQLiteAsync(query);
            }
            catch
            {
                return null;
            }
        }
        private async Task CheckAndCallUtsForTruckAsync(string logicalRef, string irsaliyeNo, string irsaliyeTarihi)
        {
            string firma = _firmaNr.PadLeft(3, '0');
            string period = _periodNo.PadLeft(2, '0');
            string sql = $@"
                SELECT 
                    SL.LOGICALREF,
                    SL.STFICHEREF,
                    SL.LINEEXP AS LOT,
                    IT.GTIN_UNO AS UNO,
                    IT.CODE AS MalzemeKodu,
                    IT.NAME AS MalzemeAdi,
                    SL.AMOUNT,
                    SL.STOCKREF
                FROM LG_{firma}_{period}_STLINE SL WITH(NOLOCK)
                INNER JOIN LG_{firma}_ITEMS IT WITH(NOLOCK) ON IT.LOGICALREF = SL.STOCKREF
                WHERE SL.STFICHEREF = @LogicalRef
                  AND SL.LINETYPE IN (0,1,8)
                  AND IT.GTIN_UNO IS NOT NULL AND IT.GTIN_UNO <> ''";
            Dictionary<string, object> prms = new Dictionary<string, object> { { "@LogicalRef", logicalRef } };
            DataTable dt = await SQLCrud.GetDataTableAsync(sql, prms);
            if (dt == null || dt.Rows.Count == 0)
            {
                XtraMessageBox.Show("Bu irsaliyede UNO tanımlı malzeme satırı bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<string> successRows = new List<string>();
            List<string> failRows = new List<string>();
            List<string> errorMessages = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                string lineRef = row["LOGICALREF"].ToString();
                string stficheRef = row["STFICHEREF"].ToString();
                string lot = row["LOT"]?.ToString() ?? "";
                string uno = row["UNO"]?.ToString() ?? "";
                string malzemeKodu = row["MalzemeKodu"]?.ToString() ?? "";
                string malzemeAdi = row["MalzemeAdi"]?.ToString() ?? "";
                int amount = row["AMOUNT"] != DBNull.Value ? Convert.ToInt32(row["AMOUNT"]) : 0;
                if (string.IsNullOrWhiteSpace(uno))
                {
                    string msg = $"❌ Malzeme '{malzemeKodu}' için UNO tanımlı değil!";
                    errorMessages.Add(msg);
                    failRows.Add(lineRef);
                    await TextLog.LogToSQLiteAsync(_userName, msg);
                    continue;
                }
                if (_isLot)
                {
                    var result = await ProcessUtsQueryWithLotAsync(
                        lineRef, stficheRef, irsaliyeNo, irsaliyeTarihi,
                        uno, amount, malzemeKodu, malzemeAdi, _userName
                    );
                    if (result.Success)
                        successRows.Add($"{malzemeKodu} (Satır: {lineRef})");
                    else
                    {
                        failRows.Add($"{malzemeKodu} (Satır: {lineRef})");
                        errorMessages.Add(result.ErrorMessage);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(lot))
                    {
                        string msg = $"❌ Malzeme '{malzemeKodu}' için LOT/LINEEXP boş!";
                        errorMessages.Add(msg);
                        failRows.Add(lineRef);
                        await TextLog.LogToSQLiteAsync(_userName, msg);
                        continue;
                    }
                    if (amount <= 0)
                    {
                        string msg = $"❌ Malzeme '{malzemeKodu}' için miktar geçersiz: {amount}";
                        errorMessages.Add(msg);
                        failRows.Add(lineRef);
                        await TextLog.LogToSQLiteAsync(_userName, msg);
                        continue;
                    }
                    string lotNo = lot.Contains("---")
                        ? lot.Split(new[] { "---" }, StringSplitOptions.None)[0].Trim()
                        : lot.Trim();
                    if (string.IsNullOrWhiteSpace(lotNo))
                    {
                        string msg = $"❌ Malzeme '{malzemeKodu}' için LOT parse edilemedi.";
                        errorMessages.Add(msg);
                        failRows.Add(lineRef);
                        await TextLog.LogToSQLiteAsync(_userName, msg);
                        continue;
                    }
                    var result = await ProcessUtsQueryForTruckAsync(
                        lineRef, stficheRef, irsaliyeNo, irsaliyeTarihi,
                        uno, lotNo, amount, malzemeKodu, malzemeAdi, _userName
                    );
                    if (result.Success)
                        successRows.Add($"{malzemeKodu} (Satır: {lineRef})");
                    else
                    {
                        failRows.Add($"{malzemeKodu} (Satır: {lineRef})");
                        errorMessages.Add(result.ErrorMessage);
                    }
                }
            }
            string resultMessage = $"═══════════════════════════════════\n";
            resultMessage += $"UTS İŞLEM SONUÇLARI - İrsaliye: {irsaliyeNo}\n";
            resultMessage += $"═══════════════════════════════════\n\n";
            resultMessage += $"✅ Başarılı: {successRows.Count} satır\n";
            resultMessage += $"❌ Başarısız: {failRows.Count} satır\n\n";
            if (successRows.Count > 0)
            {
                resultMessage += "BAŞARILI SATIRLAR:\n";
                foreach (string s in successRows)
                    resultMessage += $"  • {s}\n";
                resultMessage += "\n";
            }
            if (errorMessages.Count > 0)
            {
                resultMessage += "HATALAR:\n";
                foreach (string e in errorMessages)
                    resultMessage += $"  • {e}\n";
            }
            MessageBoxIcon icon = failRows.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning;
            XtraMessageBox.Show(resultMessage, "UTS İşlem Sonucu", MessageBoxButtons.OK, icon);
        }

        #region ==================== IsLot = 1 İÇİN METOD ====================
        private async Task<(bool Success, string ErrorMessage)> ProcessUtsQueryWithLotAsync(
            string lineRef, string stficheRef, string irsaliyeNo, string irsaliyeTarihi,
            string gtin, int amount,
            string malzemeKodu, string malzemeAdi, string userName)
        {
            try
            {
                string firma = _firmaNr.PadLeft(3, '0');
                string period = _periodNo.PadLeft(2, '0');
                string lotSql = $@"
            SELECT 
                S.CODE AS LOT,
                SLT.LOGICALREF AS SLTRANS_REF
            FROM LG_{firma}_{period}_STLINE STL WITH (NOLOCK)
            INNER JOIN LG_{firma}_ITEMS I ON STL.STOCKREF = I.LOGICALREF
            LEFT JOIN LG_{firma}_{period}_SLTRANS SLT WITH (NOLOCK) ON STL.LOGICALREF = SLT.STTRANSREF
            LEFT JOIN LG_{firma}_{period}_SERILOTN S WITH (NOLOCK) ON SLT.SLREF = S.LOGICALREF
            WHERE STL.LINETYPE IN (0,1) AND STL.CANCELLED = 0 AND STL.LOGICALREF = @LineRef";
                Dictionary<string, object> lotPrms = new Dictionary<string, object> { { "@LineRef", lineRef } };
                DataTable lotDt = await SQLCrud.GetDataTableAsync(lotSql, lotPrms);
                if (lotDt == null || lotDt.Rows.Count == 0)
                {
                    string msg = $"SERILOTN kaydı bulunamadı - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                string lotNo = lotDt.Rows[0]["LOT"]?.ToString();
                string sltransRef = lotDt.Rows[0]["SLTRANS_REF"]?.ToString();
                if (string.IsNullOrWhiteSpace(lotNo))
                {
                    string msg = $"LOT bilgisi boş - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                if (string.IsNullOrWhiteSpace(sltransRef))
                {
                    string msg = $"SLTRANS kaydı bulunamadı - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                UTSQueryResponse utsResult = await UTSApiClient.QueryAsync(gtin, lotNo, "0", amount);
                if (utsResult == null)
                {
                    string msg = $"UTS yanıt null - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                if (!utsResult.Success)
                {
                    string msg = $"UTS Hata: {utsResult.Message} - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                if (utsResult.Urun == null)
                {
                    string msg = $"UTS'de ürün bulunamadı - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                UTSUrunResponse urun = utsResult.Urun;
                // ✅ HEM ÜRETİM HEM SKT TARİHİ UPDATE EDİLİYOR
                string updateSql = $@"
            UPDATE LG_{firma}_{period}_SLTRANS 
            SET TIBBICIHAZURTDATE = @UretimTarihi,
                EXPDATE = @SonKullanmaTarihi
            WHERE LOGICALREF = @SltransRef";
                Dictionary<string, object> updatePrm = new Dictionary<string, object>
        {
            { "@UretimTarihi", string.IsNullOrEmpty(urun.UretimTarihi) ? DBNull.Value : (object)urun.UretimTarihi },
            { "@SonKullanmaTarihi", string.IsNullOrEmpty(urun.SonKullanmaTarihi) ? DBNull.Value : (object)urun.SonKullanmaTarihi },
            { "@SltransRef", sltransRef }
        };
                bool updateSuccess = await SQLCrud.ExecuteCrudAsync(updateSql, updatePrm);
                if (!updateSuccess)
                {
                    string msg = $"SLTRANS güncellenemedi - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                await InsertUtsCekimKaydiAsync(
                    lineRef, null, stficheRef, irsaliyeNo, irsaliyeTarihi,
                    gtin, lotNo, "", amount,
                    urun.UretimTarihi, urun.SonKullanmaTarihi,
                    urun.UrunTipi, urun.MarkaModel, urun.UtsStokMiktari,
                    userName
                );
                return (true, null);
            }
            catch (Exception ex)
            {
                string msg = $"HATA: {ex.Message} - {malzemeKodu}";
                await TextLog.LogToSQLiteAsync(userName, msg);
                return (false, msg);
            }
        }
        #endregion

        #region ==================== IsLot = 0 İÇİN METOD (İRSALİYE) ====================
        private async Task<(bool Success, string ErrorMessage)> ProcessUtsQueryForTruckAsync(
            string lineRef, string stficheRef, string irsaliyeNo, string irsaliyeTarihi,
            string gtin, string lotNo, int amount,
            string malzemeKodu, string malzemeAdi, string userName)
        {
            try
            {
                UTSQueryResponse utsResult = await UTSApiClient.QueryAsync(gtin, lotNo, "0", amount);
                if (utsResult == null)
                {
                    string msg = $"UTS yanıt null - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                if (!utsResult.Success)
                {
                    string msg = $"UTS Hata: {utsResult.Message} - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                if (utsResult.Urun == null)
                {
                    string msg = $"UTS'de ürün bulunamadı - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                UTSUrunResponse urun = utsResult.Urun;
                string uretimTarihiStr = string.IsNullOrEmpty(urun.UretimTarihi) ? "--" : urun.UretimTarihi;
                string sonKullanmaStr = string.IsNullOrEmpty(urun.SonKullanmaTarihi) ? "--" : urun.SonKullanmaTarihi;
                string newLineExp = $"{lotNo} --- {uretimTarihiStr} // {sonKullanmaStr}";
                string firma = _firmaNr.PadLeft(3, '0');
                string period = _periodNo.PadLeft(2, '0');
                string updateSql = $@"
                    UPDATE LG_{firma}_{period}_STLINE
                    SET LINEEXP = @P1
                    WHERE LOGICALREF = @P2";
                Dictionary<string, object> updatePrm = new Dictionary<string, object>
                {
                    { "@P1", newLineExp },
                    { "@P2", lineRef }
                };
                bool lineExpUpdated = await SQLCrud.ExecuteCrudAsync(updateSql, updatePrm);
                if (!lineExpUpdated)
                {
                    string msg = $"LINEEXP güncellenemedi - {malzemeKodu}";
                    await TextLog.LogToSQLiteAsync(userName, msg);
                    return (false, msg);
                }
                await InsertUtsCekimKaydiAsync(
                    lineRef, null, stficheRef, irsaliyeNo, irsaliyeTarihi,
                    gtin, lotNo, "", amount,
                    urun.UretimTarihi, urun.SonKullanmaTarihi,
                    urun.UrunTipi, urun.MarkaModel, urun.UtsStokMiktari,
                    userName
                );
                return (true, null);
            }
            catch (Exception ex)
            {
                string msg = $"HATA: {ex.Message} - {malzemeKodu}";
                await TextLog.LogToSQLiteAsync(userName, msg);
                return (false, msg);
            }
        }
        #endregion

        #region ==================== SQLite KAYIT ====================
        private async Task<bool> InsertUtsCekimKaydiAsync(
            string lineRef, string invoiceRef, string stficheRef,
            string faturaNo, string faturaTarihi,
            string uno, string lno, string seriNo, int miktar,
            string uretimTarihi, string sonKullanmaTarihi,
            string urunTipi, string markaModel, int utsStokMiktari,
            string userName)
        {
            try
            {
                string insertSql = @"
                    INSERT INTO UTSCekimKayitlari 
                    (STLINE_LOGICALREF, INVOICEREF, STFICHEREF, FaturaNo, FaturaTarihi, UNO, LNO, SeriNo, Miktar,
                     UretimTarihi, SonKullanmaTarihi, UrunTipi, MarkaModel, UTSStokMiktari,
                     UTSdenCekildi, LogoKayitAtildi, KayitTarihi, KullaniciAdi)
                    VALUES 
                    (@LineRef, @InvoiceRef, @StficheRef, @FaturaNo, @FaturaTarihi, @UNO, @LNO, @SeriNo, @Miktar,
                     @UretimTarihi, @SKT, @UrunTipi, @MarkaModel, @UTSStok,
                     1, 1, @KayitTarihi, @Kullanici)";
                Dictionary<string, object> prm = new Dictionary<string, object>
                {
                    { "@LineRef", lineRef },
                    { "@InvoiceRef", invoiceRef ?? (object)DBNull.Value },
                    { "@StficheRef", stficheRef ?? (object)DBNull.Value },
                    { "@FaturaNo", faturaNo ?? "" },
                    { "@FaturaTarihi", faturaTarihi ?? "" },
                    { "@UNO", uno },
                    { "@LNO", lno },
                    { "@SeriNo", seriNo ?? "" },
                    { "@Miktar", miktar },
                    { "@UretimTarihi", uretimTarihi ?? "" },
                    { "@SKT", sonKullanmaTarihi ?? "" },
                    { "@UrunTipi", urunTipi ?? "" },
                    { "@MarkaModel", markaModel ?? "" },
                    { "@UTSStok", utsStokMiktari },
                    { "@KayitTarihi", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "@Kullanici", userName }
                };
                var success = await SQLiteCrud.InsertUpdateDeleteAsync(insertSql, prm);
                return success.Success;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(userName, $"SQLite kayıt HATA: {ex.Message}");
                return false;
            }
        }
        #endregion

    }
}
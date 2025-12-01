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
    public partial class SalesInvoicesForm : XtraForm
    {
        private string _userName;
        private string _firmaNr;
        private string _periodNo;

        public SalesInvoicesForm(string userName, string firmaNr, string periodNo)
        {
            InitializeComponent();
            _userName = userName;
            _firmaNr = firmaNr;
            _periodNo = periodNo;
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
        }
        // SalesInvoicesForm.cs içinde olması gereken örnek metot
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs args = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(args.Location);

            if (hitInfo.InRow || hitInfo.InRowCell)
            {
                // 1. LogicalRef'i al
                var logicalRef = view.GetFocusedRowCellValue("LOGICALREF")?.ToString();

                if (!string.IsNullOrEmpty(logicalRef))
                {
                    // 2. Yeni formu aç (Bu constructor'ı kullandık)
                    SalesDetailsForm detailForm = new SalesDetailsForm(
                        logicalRef,
                        _userName,
                        _firmaNr,
                        _periodNo // Ana formdaki field'ları kullanarak bilgileri gönder
                    );
                    detailForm.ShowDialog();
                }
            }
        }
        private async void SalesInvoicesForm_Load(object sender, EventArgs e)
        {
            await LoadSalesInvoicesAsync();
        }

        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await LoadSalesInvoicesAsync();
        }

        private async void UTSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRow() == null)
            {
                XtraMessageBox.Show("Lütfen bir fatura seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var logicalRef = gridView1.GetFocusedRowCellValue("LOGICALREF")?.ToString();
            var faturaNo = gridView1.GetFocusedRowCellValue("Fatura No")?.ToString();
            var faturaTarihi = gridView1.GetFocusedRowCellValue("Fatura Tarihi")?.ToString();
            if (string.IsNullOrEmpty(logicalRef)) return;
            string utsDurumu = gridView1.GetFocusedRowCellValue("UTS Durumu")?.ToString();
            if (utsDurumu == "✓ Çekildi")
            {
                var result = XtraMessageBox.Show(
                    $"Bu fatura daha önce UTS'den çekilmiş.\n\nTekrar çekmek istiyor musunuz?",
                    "Uyarı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;
            }
            await CheckAndCallUtsForInvoiceAsync(logicalRef, faturaNo, faturaTarihi);
            await LoadSalesInvoicesAsync();
        }
        private async Task LoadSalesInvoicesAsync()
        {
            try
            {
                string firma = _firmaNr.PadLeft(3, '0');
                string period = _periodNo.PadLeft(2, '0');
                DataTable utsKayitlar = await GetUtsCekilmisFaturalarAsync();
                HashSet<string> cekilmisFaturalar = new HashSet<string>();

                if (utsKayitlar != null && utsKayitlar.Rows.Count > 0)
                {
                    foreach (DataRow row in utsKayitlar.Rows)
                    {
                        string invRef = row["INVOICEREF"]?.ToString();
                        if (!string.IsNullOrEmpty(invRef))
                            cekilmisFaturalar.Add(invRef);
                    }
                }

                string query = $@"
                  SELECT  
    -- Fatura Tipini Belirleme
    CASE 
        WHEN INV.EINVOICE = 1 THEN 'E-Fatura'
        WHEN INV.EINVOICE = 2 THEN 'E-Arşiv'
        ELSE 'Diğer'
    END AS [FaturaTipi],

    -- Durum Alanını Belirleme (E-Fatura ve E-Arşiv için farklı mantık)
    CASE 
        -- E-Arşiv Fatura (INV.EINVOICE = 2) için EAR.EARCHIVESTATUS'a göre durum
        WHEN INV.EINVOICE = 2 THEN
            CASE EAR.EARCHIVESTATUS
                WHEN 0 THEN 'GİB Gönderilecek'
                WHEN 1 THEN 'Onaylandı'
                ELSE 'Bilinmiyor (E-Arşiv)'
            END
        -- E-Fatura (INV.EINVOICE = 1) için INV.ESTATUS'a göre durum (Orijinal sorgudan INV.EINVOICE=1 durumunda ESTATUS kullanıldığı anlaşılıyor, ancak burada ESTATUS yerine E-Fatura için genel EINVOICE durumları kullanıldı.)
        WHEN INV.EINVOICE = 1 THEN
            CASE INV.ESTATUS -- Normalde ESTATUS kullanılmalı, ancak orijinal sorgunuzdaki CASE yapısı INV.EINVOICE'i kullanmış. E-Fatura için ESTATUS'ü tercih etmek daha doğrudur.
                WHEN 0 THEN 'GİB''e Gönderilecek'
                WHEN 1 THEN 'Onay Gönderildi'
                WHEN 2 THEN 'Onaylandı'
                WHEN 3 THEN 'Paketlendi'
                ELSE 'Bilinmiyor (E-Fatura)'
            END
        ELSE 'Bilinmiyor'
    END AS [Durum],
    INV.LOGICALREF,
    INV.DATE_ AS [FaturaTarihiRaw],
    CONVERT(VARCHAR(20), INV.DATE_, 104) AS [Fatura Tarihi],
    INV.FICHENO AS [Fatura No],
    CL.CODE AS [Cari Kodu],
    CL.DEFINITION_ AS [Cari Adı],
    INV.GENEXP1 AS [Fatura Açıklama 1],
    INV.GENEXP2 AS [Fatura Açıklama 2],
    INV.VAT AS [KDV Oranı],
    INV.TOTALVAT AS [Toplam KDV],
    INV.GROSSTOTAL AS [Brüt Toplam],
    INV.NETTOTAL AS [TL Tutarı],
    INV.TRNET AS [Döviz Tutarı],
    ISNULL(CURR.CURCODE, 'TL') AS [Döviz Türü],
    USERS.NAME AS [Kaydı Yapan]
FROM 
    LG_{firma}_{period}_INVOICE INV WITH(NOLOCK)
LEFT JOIN 
    LG_{firma}_CLCARD CL WITH(NOLOCK) ON CL.LOGICALREF = INV.CLIENTREF
LEFT JOIN 
    L_CURRENCYLIST CURR WITH(NOLOCK) ON CURR.CURTYPE = INV.TRCURR AND CURR.FIRMNR = {firma}
LEFT JOIN 
    L_CAPIUSER USERS WITH(NOLOCK) ON USERS.NR = INV.CAPIBLOCK_CREATEDBY
LEFT JOIN 
    LG_{firma}_{period}_EARCHIVEDET EAR WITH (NOLOCK) ON EAR.INVOICEREF = INV.LOGICALREF
WHERE 
    INV.GRPCODE = 2          
    AND INV.CANCELLED = 0     
    AND INV.PROFILEID = 8    
    AND (INV.ESTATUS IN (0,1,2,3))
    AND (
        (INV.EINVOICE = 1 AND INV.ESTATUS IN (0,1,2,3)) OR -- E-Fatura için ESTATUS kontrolü
        (INV.EINVOICE = 2 AND EAR.EARCHIVESTATUS IN (0,1)) -- E-Arşiv için EAR.EARCHIVESTATUS kontrolü (Orijinalde 0,1 istenmişti, 2 de eklendi)
    )
ORDER BY 
    INV.LOGICALREF DESC";

                DataTable dt = await SQLCrud.GetDataTableAsync(query, null);
                dt.Columns.Add("UTS Durumu", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    string logicalRef = row["LOGICALREF"]?.ToString();
                    if (cekilmisFaturalar.Contains(logicalRef))
                    {
                        row["UTS Durumu"] = "✓ Çekildi";
                    }
                    else
                    {
                        row["UTS Durumu"] = "Çekilmedi";
                    }
                }
                gridControl1.DataSource = dt;
                if (gridView1.Columns["LOGICALREF"] != null)
                    gridView1.Columns["LOGICALREF"].Visible = false;
                if (gridView1.Columns["FaturaTarihiRaw"] != null)
                    gridView1.Columns["FaturaTarihiRaw"].Visible = false;
                if (gridView1.Columns["UTS Durumu"] != null)
                {
                    gridView1.Columns["UTS Durumu"].VisibleIndex = 0;
                }
                gridView1.BestFitColumns();
                GridViewDesigner.CustomizeGrid(gridView1);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Faturalar yüklenemedi:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(_userName, $"Fatura Listeleme Hatası: {ex.Message}");
            }
        }
        private async Task<DataTable> GetUtsCekilmisFaturalarAsync()
        {
            try
            {
                string query = "SELECT DISTINCT INVOICEREF FROM UTSCekimKayitlari WHERE UTSdenCekildi = 1";
                return await SQLiteCrud.GetDataFromSQLiteAsync(query);
            }
            catch
            {
                return null;
            }
        }
        private async Task CheckAndCallUtsForInvoiceAsync(string logicalRef, string faturaNo, string faturaTarihi)
        {
            string firma = _firmaNr.PadLeft(3, '0');
            string period = _periodNo.PadLeft(2, '0');
            string sql = $@"
                SELECT 
                    SL.LOGICALREF,
                    SL.INVOICEREF,
                    SL.LINEEXP AS LOT,
                    IT.GTIN_UNO AS UNO,
                    IT.CODE AS MalzemeKodu,
                    IT.NAME AS MalzemeAdi,
                    SL.AMOUNT,
                    SL.STOCKREF
                FROM LG_{firma}_{period}_STLINE SL WITH(NOLOCK)
                INNER JOIN LG_{firma}_ITEMS IT WITH(NOLOCK) ON IT.LOGICALREF = SL.STOCKREF
                WHERE SL.INVOICEREF = @LogicalRef
                  AND SL.LINETYPE IN (0,1,8)
                  AND IT.GTIN_UNO IS NOT NULL AND IT.GTIN_UNO <> ''";
            Dictionary<string, object> prms = new Dictionary<string, object> { { "@LogicalRef", logicalRef } };
            DataTable dt = await SQLCrud.GetDataTableAsync(sql, prms);
            if (dt == null || dt.Rows.Count == 0)
            {
                XtraMessageBox.Show("Bu faturada UNO tanımlı malzeme satırı bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<string> successRows = new List<string>();
            List<string> failRows = new List<string>();
            List<string> errorMessages = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                string lineRef = row["LOGICALREF"].ToString();
                string invoiceRef = row["INVOICEREF"].ToString();
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
                var result = await ProcessUtsQueryAsync(
                    lineRef, invoiceRef, faturaNo, faturaTarihi,
                    uno, lotNo, amount, malzemeKodu, malzemeAdi, _userName
                );
                if (result.Success)
                {
                    successRows.Add($"{malzemeKodu} (Satır: {lineRef})");
                }
                else
                {
                    failRows.Add($"{malzemeKodu} (Satır: {lineRef})");
                    errorMessages.Add(result.ErrorMessage);
                }
            }
            string resultMessage = $"═══════════════════════════════════\n";
            resultMessage += $"UTS İŞLEM SONUÇLARI - Fatura: {faturaNo}\n";
            resultMessage += $"═══════════════════════════════════\n\n";
            resultMessage += $"✅ Başarılı: {successRows.Count} satır\n";
            resultMessage += $"❌ Başarısız: {failRows.Count} satır\n\n";
            if (successRows.Count > 0)
            {
                resultMessage += "BAŞARILI SATIRLAR:\n";
                foreach (var s in successRows)
                    resultMessage += $"  • {s}\n";
                resultMessage += "\n";
            }
            if (errorMessages.Count > 0)
            {
                resultMessage += "HATALAR:\n";
                foreach (var e in errorMessages)
                    resultMessage += $"  • {e}\n";
            }
            MessageBoxIcon icon = failRows.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning;
            XtraMessageBox.Show(resultMessage, "UTS İşlem Sonucu", MessageBoxButtons.OK, icon);
        }
        private async Task<(bool Success, string ErrorMessage)> ProcessUtsQueryAsync(
            string lineRef, string invoiceRef, string faturaNo, string faturaTarihi,
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
                string newLineExp = (!string.IsNullOrEmpty(urun.UretimTarihi) && !string.IsNullOrEmpty(urun.SonKullanmaTarihi))
                    ? $"{lotNo} --- {urun.UretimTarihi} // {urun.SonKullanmaTarihi}"
                    : lotNo;
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
                await InsertPtsNoticeRecordsAsync(
                    firma, period, faturaNo, faturaTarihi,
                    gtin, lotNo, urun.UretimTarihi, urun.SonKullanmaTarihi,
                    amount, userName
                );
                await InsertUtsCekimKaydiAsync(
                    lineRef, invoiceRef, faturaNo, faturaTarihi,
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
        private async Task<bool> InsertPtsNoticeRecordsAsync(
            string firma, string period,
            string faturaNo, string faturaTarihi,
            string gtin, string lotNo,
            string uretimTarihi, string sonKullanmaTarihi,
            int amount, string userName)
        {
            try
            {
                DateTime faturaDate;
                if (!DateTime.TryParse(faturaTarihi, out faturaDate))
                    faturaDate = DateTime.Now;
                for (int i = 0; i < amount; i++)
                {
                    string insertSql = $@"
                        INSERT INTO LG_{firma}_PTSNOTICE 
                        (DOCNUMBER, DOCDATE, GTIN, LOTNUMBER, EXPIRATIONDATE, SERIALNUMBER, PRODUCTIONDATE, 
                         CAPIBLOCK_CREATEDBY, CAPIBLOCK_CREADEDDATE, CAPIBLOCK_CREATEDHOUR, CAPIBLOCK_CREATEDMIN, CAPIBLOCK_CREATEDSEC,
                         CAPIBLOCK_MODIFIEDBY, CAPIBLOCK_MODIFIEDDATE, CAPIBLOCK_MODIFIEDHOUR, CAPIBLOCK_MODIFIEDMIN, CAPIBLOCK_MODIFIEDSEC)
                        VALUES 
                        (@FaturaNo, @FaturaTarihi, @GTIN, @LotNo, @SKT, '', @URT,
                         1, GETDATE(), DATEPART(HOUR, GETDATE()), DATEPART(MINUTE, GETDATE()), DATEPART(SECOND, GETDATE()),
                         NULL, NULL, NULL, NULL, NULL)";
                    Dictionary<string, object> insertPrm = new Dictionary<string, object>
                    {
                        { "@FaturaNo", faturaNo },
                        { "@FaturaTarihi", faturaDate },
                        { "@GTIN", gtin },
                        { "@LotNo", lotNo },
                        { "@SKT", string.IsNullOrEmpty(sonKullanmaTarihi) ? DBNull.Value : (object)sonKullanmaTarihi },
                        { "@URT", string.IsNullOrEmpty(uretimTarihi) ? DBNull.Value : (object)uretimTarihi }
                    };
                    bool success = await SQLCrud.ExecuteCrudAsync(insertSql, insertPrm);
                    if (!success)
                    {
                        await TextLog.LogToSQLiteAsync(userName, $"PTS Notice insert hatası - Kayıt {i + 1}/{amount}");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync(userName, $"PTS Notice HATA: {ex.Message}");
                return false;
            }
        }
        private async Task<bool> InsertUtsCekimKaydiAsync(
            string lineRef, string invoiceRef,
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
                    (STLINE_LOGICALREF, INVOICEREF, FaturaNo, FaturaTarihi, UNO, LNO, SeriNo, Miktar,
                     UretimTarihi, SonKullanmaTarihi, UrunTipi, MarkaModel, UTSStokMiktari,
                     UTSdenCekildi, LogoKayitAtildi, KayitTarihi, KullaniciAdi)
                    VALUES 
                    (@LineRef, @InvoiceRef, @FaturaNo, @FaturaTarihi, @UNO, @LNO, @SeriNo, @Miktar,
                     @UretimTarihi, @SKT, @UrunTipi, @MarkaModel, @UTSStok,
                     1, 1, @KayitTarihi, @Kullanici)";
                Dictionary<string, object> prm = new Dictionary<string, object>
                {
                    { "@LineRef", lineRef },
                    { "@InvoiceRef", invoiceRef },
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
    }
}
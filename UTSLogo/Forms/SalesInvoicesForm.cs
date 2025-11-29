using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid; // GridView için gerekli olabilir
using UTSLogo.Classes;
using UTSLogo.Models;

namespace UTSLogo.Forms
{
    // Varsayım: Bu formun görsel bileşenleri (gridControl1, gridView1, refreshToolStripMenuItem, UTSToolStripMenuItem) tasarlanmıştır.
    public partial class SalesInvoicesForm : XtraForm
    {
        private string _userName = "";
        private string _firmaNr = "";
        private string _periodNo = "";

        // --- Constructor ---
        public SalesInvoicesForm(string userName, string firmaNr, string periodNo)
        {
            InitializeComponent();
            _userName = userName;
            _firmaNr = firmaNr;
            _periodNo = periodNo;
        }

        // --- Event Handlers ---

        private async void SalesInvoicesForm_Load(object sender, EventArgs e)
        {
            await LoadSalesInvoicesAsync();
        }

        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await LoadSalesInvoicesAsync();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRow() == null) return;
            var logicalRef = gridView1.GetFocusedRowCellValue("LOGICALREF")?.ToString();
            if (string.IsNullOrEmpty(logicalRef)) return;

            // SalesDetailsForm'un LOGICALREF'i kabul ettiğini varsayıyoruz.
            // SalesDetailsForm detailsForm = new SalesDetailsForm(logicalRef);
            // detailsForm.ShowDialog();
        }

        private async void UTSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRow() == null)
            {
                XtraMessageBox.Show("Lütfen öncelikle listeden bir fatura seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var logicalRef = gridView1.GetFocusedRowCellValue("LOGICALREF")?.ToString();
            if (string.IsNullOrEmpty(logicalRef))
            {
                XtraMessageBox.Show("Seçili faturanın LOGICALREF bilgisine ulaşılamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // İşlemi asenkron olarak başlat
            await CheckAndCallUtsForInvoiceAsync(logicalRef);
        }

        // --- Core Methods ---

        /// <summary>
        /// Ana Fatura Listesini SQL Server'dan çeker ve GridControl'e bağlar.
        /// </summary>
        private async Task LoadSalesInvoicesAsync()
        {
            try
            {
                // Kullanıcı ve firma/dönem bilgileri zaten Constructor'da set edildiği için sadece formatlayalım
                string firmaFilter = _firmaNr.PadLeft(3, '0');
                string periodFilter = _periodNo.PadLeft(2, '0');

                // SQL Server sorgusu (Daha önce paylaştığınız sorgunun başlık kısmı)
                string query = $@"
                    SELECT 
                        CASE INV.ESTATUS
                            WHEN 0 THEN 'GİB’e Gönderilecek'
                            WHEN 1 THEN 'Onay Gönderildi'
                            WHEN 2 THEN 'Onaylandı'
                            WHEN 3 THEN 'Paketlendi'
                            ELSE 'Diğer Durumlar'
                        END AS [Durum],
                        INV.LOGICALREF,
                        CONVERT(VARCHAR(20), INV.DATE_, 104) AS [Fatura Tarihi],
                        INV.FICHENO AS [Fatura No],
                        CL.CODE AS [Cari Kodu],
                        CL.DEFINITION_ AS [Cari Adı],
                        INV.NETTOTAL AS [TL Tutarı],
                        INV.TRNET AS [Döviz Tutarı],
                        CASE WHEN CURR.CURCODE IS NULL THEN 'TL' ELSE CURR.CURCODE END AS [Döviz Türü],
                        USERS.NAME AS [Kaydı Yapan]
                    FROM LG_{firmaFilter}_{periodFilter}_INVOICE INV WITH (NOLOCK)
                    LEFT JOIN LG_{firmaFilter}_CLCARD CL WITH (NOLOCK) ON CL.LOGICALREF = INV.CLIENTREF
                    LEFT JOIN L_CURRENCYLIST CURR WITH (NOLOCK) ON CURR.CURTYPE = INV.TRCURR AND CURR.FIRMNR = {firmaFilter}
                    JOIN L_CAPIUSER USERS WITH (NOLOCK) ON USERS.NR = INV.CAPIBLOCK_CREATEDBY 
                    WHERE INV.TRCODE IN (7,8,9) -- Belirlenen Satış Fatura Tipleri
                      AND INV.CANCELLED=0
                      AND INV.ESTATUS IN (0, 1, 2, 3)
                    ORDER BY INV.LOGICALREF DESC";

                // SQLCrud sınıfının GetDataTableAsync metodu üzerinden veri çekilir (varsayım)
                DataTable dtInvoices = await SQLCrud.GetDataTableAsync(query, null);

                gridControl1.DataSource = dtInvoices;

                // GridView tasarım ayarları
                GridViewDesigner.CustomizeGrid(gridView1); // Varsayım: Harici bir tasarım sınıfı
                if (gridView1.Columns["LOGICALREF"] != null)
                    gridView1.Columns["LOGICALREF"].Visible = false;

                gridView1.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Faturalar yüklenemedi:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Seçili faturanın satırlarını kontrol eder, UTS'e sorgular ve LINEEXP alanını günceller.
        /// </summary>
        private async Task CheckAndCallUtsForInvoiceAsync(string logicalRef)
        {
            string firmaFilter = _firmaNr.PadLeft(3, '0');
            string periodFilter = _periodNo.PadLeft(2, '0');

            // LOGO'dan (SQL Server) fatura satırlarını çeken sorgu
            string lineQuery = $@"
                SELECT 
                    SLT.LOGICALREF,
                    SLT.LINEEXP 'LOT',
                    ITM.GTIN_UNO 'GTN',
                    SLT.AMOUNT,
                    SLT.STOCKREF
                FROM LG_{firmaFilter}_{periodFilter}_STLINE SLT WITH (NOLOCK)
                JOIN LG_{firmaFilter}_ITEMS ITM WITH (NOLOCK) ON ITM.LOGICALREF = SLT.STOCKREF
                WHERE SLT.INVOICEREF = @LogicalRef
                AND SLT.LINETYPE IN (0, 1, 8)"; // Sadece Stok Satırları (0:Mal, 1:Ek maliyet, 8:Promosyon/Hediye)

            var parameters = new Dictionary<string, object> { { "@LogicalRef", logicalRef } };

            DataTable dtLines;
            try
            {
                // SQL Server'dan veri çekme (SQLCrud'un varlığı varsayılır)
                dtLines = await SQLCrud.GetDataTableAsync(lineQuery, parameters);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"LOGO Satır verileri yüklenemedi: {ex.Message}", "LOGO Veri Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(_userName, $"LOGO Satır Çekme Hatası (LogicalRef: {logicalRef}): {ex.Message}");
                return;
            }

            if (dtLines == null || dtLines.Rows.Count == 0)
            {
                XtraMessageBox.Show("Seçilen faturada UTS kontrolü yapılacak stok satırı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var updateTasks = new List<Task>();
            var errorMessages = new List<string>();

            // Her bir satırı döngüye al
            foreach (DataRow row in dtLines.Rows)
            {
                var lineRef = row["LOGICALREF"]?.ToString();
                var lotRaw = row["LOT"]?.ToString();
                var gtin = row["GTN"]?.ToString();
                // SQL'den gelen miktar decimal olarak okunur
                var amount = row["AMOUNT"] != DBNull.Value ? Convert.ToDecimal(row["AMOUNT"]) : 0m;

                // 1. Veri Kontrolü (Boş/Sıfır Kontrolü)
                if (string.IsNullOrWhiteSpace(lotRaw) || string.IsNullOrWhiteSpace(gtin) || amount <= 0)
                {
                    errorMessages.Add($"Satır LOGICALREF {lineRef}: Lot ({lotRaw}), GTIN ({gtin}) veya Miktar ({amount}) boş/sıfır olamaz. Lütfen LOGO'dan doldurun.");
                    continue;
                }

                // 2. LOT Verisinin Ayrıştırılması
                string lotNo;
                if (lotRaw.Contains("---"))
                {
                    // Eğer LINEEXP içinde URT/SKT bilgisi varsa, '---' solundaki Lot Numarasıdır.
                    lotNo = lotRaw.Split(new[] { "---" }, StringSplitOptions.None)[0].Trim();
                }
                else
                {
                    lotNo = lotRaw.Trim();
                }

                // 3. UTS API'ye Sorgu Gönderme ve Sonuçları İşleme (Asenkron)
                // Decimal miktarı API'nin int ihtiyacına karşılık int'e çeviriyoruz
                updateTasks.Add(ProcessUtsQueryAsync(lineRef, gtin, lotNo, (int)amount, _userName));
            }

            // Tüm asenkron işlemleri bekle
            await Task.WhenAll(updateTasks);

            // Toplanan hataları göster
            if (errorMessages.Count > 0)
            {
                XtraMessageBox.Show(
                    "Bazı satırlarda eksik bilgi bulundu. Lütfen LOGO'dan düzeltin:\n\n" + string.Join("\n", errorMessages),
                    "Eksik Bilgi Uyarısı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            XtraMessageBox.Show("UTS kontrol ve güncelleme işlemleri tamamlandı. Sonuçlar için logları kontrol edin.", "İşlem Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Tek bir fatura satırı için UTS sorgusu yapar, miktarı kontrol eder ve LINEEXP'i günceller.
        /// </summary>
        private async Task ProcessUtsQueryAsync(string lineRef, string gtin, string lotNo, int logoAmount, string userName)
        {
            string logMessage = $"Satır {lineRef}, UNO: {gtin}, LNO: {lotNo}";

            try
            {
                // UNO=GTIN, LNO=LOT, SAN=0 (Seri No her zaman 0 olarak gönderiliyor)
                var utsResult = await UTSApiClient.SorgulaTekilUrunAsync(gtin, lotNo, "0", userName);

                // API bağlantı veya token hatası varsa utsResult null döner
                if (utsResult == null) return;

                if (utsResult.UrunListesi == null || utsResult.UrunListesi.Count == 0)
                {
                    XtraMessageBox.Show($"UTS Sorgu Başarısız ({logMessage}): Ürün kaydı bulunamadı.", "UTS Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await TextLog.LogToSQLiteAsync(userName, $"UTS API Yanıtı Boş: {logMessage}");
                    return;
                }

                var urunDetayi = utsResult.UrunListesi[0];
                int utsAvailableAmount = urunDetayi.Adet; // ADT alanını kullanıyoruz

                // 4. Miktar Kontrolü
                if (logoAmount > utsAvailableAmount)
                {
                    XtraMessageBox.Show(
                        $"UTS Stok Yetersiz! ({logMessage}): LOGO Miktar: {logoAmount}, UTS Kullanılabilir: {utsAvailableAmount}. Çıkış yapılamaz.",
                        "Stok Yetersiz",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop
                    );
                    await TextLog.LogToSQLiteAsync(userName, $"Miktar Yet. Değil: {logMessage}, LOGO: {logoAmount}, UTS: {utsAvailableAmount}");
                    return;
                }

                // 5. LINEEXP Alanını Güncelleme
                // Yeni Format: LOT --- URT Tarihi // SKT Tarihi (Örn: LOT123 --- 2024-01-01 // 2025-01-01)
                string newUrtSktPart = $"--- {urunDetayi.UretimTarihi:yyyy-MM-dd} // {urunDetayi.SonKullanmaTarihi:yyyy-MM-dd}";
                string newLineExp = $"{lotNo} {newUrtSktPart}";

                // LINEEXP Güncelleme Sorgusu
                string updateQuery = $@"
                    UPDATE LG_{_firmaNr.PadLeft(3, '0')}_{_periodNo.PadLeft(2, '0')}_STLINE
                    SET LINEEXP = @NewLineExp
                    WHERE LOGICALREF = @LineRef";

                var updateParams = new Dictionary<string, object>
                {
                    { "@NewLineExp", newLineExp },
                    { "@LineRef", lineRef }
                };

                // SQLCrud'un InsertUpdateDeleteAsync metodu üzerinden güncelleme yapılır (varsayım)
                var updateResult = await SQLCrud.ExecuteCrudAsync(updateQuery, updateParams);

                if (updateResult)
                {
                    await TextLog.LogToSQLiteAsync(userName, $"STLINE Güncelleme Başarılı: {logMessage}, Yeni LINEEXP: {newLineExp}");
                }
                else
                {
                    XtraMessageBox.Show($"STLINE Güncelleme Hatası ({logMessage}): {updateResult}", "DB Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"UTS İşleme Genel Hata ({logMessage}): {ex.Message}", "İşlem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(userName, $"UTS İşleme Genel Hata: {logMessage}, Exception: {ex.Message}");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid; 
using UTSLogo.Classes;

namespace UTSLogo.Forms
{
    public partial class SalesDetailsForm : XtraForm
    {
        private string _logicalRef;
        private string _userName;
        private string _firmaNr;
        private string _periodNo;
        public SalesDetailsForm(string logicalRef, string userName, string firmaNr, string periodNo)
        {
            InitializeComponent();
            _logicalRef = logicalRef;
            _userName = userName;
            _firmaNr = firmaNr;
            _periodNo = periodNo;
            this.Text = "Fatura Satır Detayları Yükleniyor...";
        }
        private async void SalesDetailsForm_Load(object sender, EventArgs e)
        {
            await LoadSalesDetailsAsync();
        }
        private async Task LoadSalesDetailsAsync()
        {
            try
            {
                string firmaFilter = _firmaNr.PadLeft(3, '0');
                string periodFilter = _periodNo.PadLeft(2, '0');
                string query = $@"
SELECT 
    STLINE.LINEEXP AS [Satır Açıklaması (LOT)],
	ITEMS.GTIN_UNO AS [UNO],
    CASE 
        WHEN STLINE.LINETYPE = 4 THEN SRVCARD.CODE
        WHEN STLINE.LINETYPE = 0 THEN ITEMS.CODE
        WHEN STLINE.LINETYPE = 2 THEN NULL
        ELSE ITEMS.CODE 
    END AS [Stok Kodu],
    CASE WHEN STLINE.LINETYPE = 2 THEN 'İNDİRİM' ELSE NULL END AS [Hizmet Kodu],
    CASE 
        WHEN STLINE.LINETYPE = 4 THEN SRVCARD.DEFINITION_
        WHEN STLINE.LINETYPE = 2 THEN 'İndirim Satırı'
        ELSE ITEMS.NAME 
    END AS [Stok Adı],
    UNITSETL.CODE AS [Birim Kodu],
    CASE WHEN STLINE.LINETYPE = 4 THEN SRVCARD.DEFINITION_ ELSE NULL END AS [Hizmet Açıklaması],
    CASE WHEN STLINE.LINETYPE = 2 THEN NULL ELSE ROUND(STLINE.AMOUNT,3) END AS [Miktar],
    STLINE.PRICE AS [Birim Fiyat],
    STLINE.VAT AS [KDV Oranı],
    ROUND(STLINE.VATAMNT,3) AS [KDV Tutarı],
    ROUND(STLINE.VATMATRAH,3) AS [KDV Matrahı], 
    CASE WHEN STLINE.LINETYPE = 2 THEN NULL ELSE ROUND(STLINE.LINENET,3) END AS [Net Satış Tutarı],
    ID_KUR.CURCODE AS [İşlem Döviz Türü],
    CASE WHEN STLINE.LINETYPE = 2 THEN 0 ELSE CASE WHEN STLINE.TRRATE=0 THEN 0 ELSE ROUND(STLINE.VATAMNT/STLINE.TRRATE,3) END END AS [İşlem Döviz KDV],
    CASE WHEN STLINE.LINETYPE = 2 THEN 0 ELSE CASE WHEN STLINE.TRRATE=0 THEN 0 ELSE ROUND(STLINE.TOTAL/STLINE.TRRATE,3) END END AS [İşlem Döviz Tutarı],
    CASE WHEN STLINE.LINETYPE = 2 THEN 0 ELSE CASE WHEN STLINE.TRRATE=0 THEN 0 ELSE ROUND(STLINE.LINENET/STLINE.TRRATE,3) END END AS [İşlem Döviz Net Tutarı],
    STLINE.DISCPER AS [İndirim Oranı]
FROM 
    LG_{firmaFilter}_{periodFilter}_STLINE STLINE WITH(NOLOCK)
LEFT JOIN 
    LG_{firmaFilter}_{periodFilter}_STFICHE STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
LEFT JOIN 
    LG_{firmaFilter}_{periodFilter}_INVOICE INVOICE WITH(NOLOCK) ON INVOICE.LOGICALREF = STLINE.INVOICEREF
LEFT JOIN 
    LG_{firmaFilter}_CLCARD CLCARD WITH(NOLOCK) ON CLCARD.LOGICALREF = STFICHE.CLIENTREF
LEFT JOIN 
    LG_{firmaFilter}_ITEMS ITEMS WITH(NOLOCK) ON ITEMS.LOGICALREF = STLINE.STOCKREF AND STLINE.LINETYPE IN (0, 8, 1)
LEFT JOIN 
    LG_{firmaFilter}_SRVCARD SRVCARD WITH(NOLOCK) ON SRVCARD.LOGICALREF = STLINE.STOCKREF AND STLINE.LINETYPE = 4
LEFT JOIN 
    LG_{firmaFilter}_UNITSETL UNITSETL WITH(NOLOCK) ON STLINE.UOMREF = UNITSETL.LOGICALREF
LEFT JOIN 
    L_CURRENCYLIST ID_KUR WITH(NOLOCK) ON ID_KUR.CURTYPE = STLINE.TRCURR AND ID_KUR.FIRMNR = '{firmaFilter}'
WHERE 
    INVOICE.CANCELLED=0 
    AND INVOICE.LOGICALREF=@LogicalRef
    AND STLINE.LINETYPE IN (0,1,2,4,8,11)
    AND INVOICE.TRCODE IN (2,3,7,8,9,10,14)
    AND (STFICHE.BILLED=1 OR STFICHE.BILLED IS NULL)
ORDER BY STLINE.LOGICALREF ASC";
                var parameters = new Dictionary<string, object> { { "@LogicalRef", _logicalRef } };
                DataTable dtDetails = await SQLCrud.GetDataTableAsync(query, parameters);
                if (dtDetails == null || dtDetails.Rows.Count == 0)
                {
                    gridControl1.DataSource = null;
                    this.Text = "Fatura Satır Detayları (Kayıt Bulunamadı)";
                    return;
                }
                gridControl1.DataSource = dtDetails;
                this.Text = $"Fatura Detayları (LogicalRef: {_logicalRef}) - Satır Sayısı: {dtDetails.Rows.Count}";
                GridView gridView = (GridView)gridControl1.Views[0];
                if (gridView.Columns["LOGICALREF"] != null)
                    gridView.Columns["LOGICALREF"].Visible = false;
                if (gridView.Columns["Satır Açıklaması (LOT)"] != null)
                    gridView.Columns["Satır Açıklaması (LOT)"].VisibleIndex = 0;
                if (gridView.Columns["GTIN/UNO"] != null)
                    gridView.Columns["GTIN/UNO"].VisibleIndex = 1;
                gridView.OptionsView.ColumnAutoWidth = false;
                gridView.OptionsBehavior.Editable = false;
                gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
                gridView.BestFitColumns();
                GridViewDesigner.CustomizeGrid(gridView1);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Fatura detayları yüklenemedi:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await TextLog.LogToSQLiteAsync(_userName, $"Fatura Detay Yükleme Hatası (Ref: {_logicalRef}): {ex.Message}");
            }
        }
    }
}
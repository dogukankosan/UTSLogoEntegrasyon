using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using UTSLogo.Classes;
namespace UTSLogo.Forms
{
    public partial class FormCompanyChoose : XtraForm
    {
        public FormCompanyChoose()
        {
            InitializeComponent();
        }
        public string companyNr = "", companyName = "", periodNo = "";
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                string sirketKodu = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "Firma Numarası")?.ToString();
                string sirketDonem = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "Dönem No")?.ToString();
                string sirketAdi = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "Firma Adı")?.ToString();
                if (!string.IsNullOrEmpty(sirketKodu))
                {
                    companyNr = sirketKodu;
                    companyName = sirketAdi;
                    periodNo = sirketDonem;
                    this.Close();
                }
            }
        }
        private async void FormCompanyChoose_Load(object sender, EventArgs e)
        {
            DataTable dt = await SQLCrud.GetDataTableAsync(@"SELECT 
    FIRM.NR AS 'Firma Numarası',
    FIRM.NAME AS 'Firma Adı',
    FIRM.TITLE AS 'Firma Ünvanı',
    PERI.NR 'Dönem No',
    CONVERT(VARCHAR(50),PERI.BEGDATE,104) AS 'Dönem Başlangıç Tarihi',
    CONVERT(VARCHAR(50),PERI.ENDDATE,104) AS 'Dönem Bitiş Tarihi'
FROM 
    L_CAPIFIRM FIRM WITH (NOLOCK)
JOIN 
    L_CAPIPERIOD PERI WITH (NOLOCK) 
ON 
    PERI.FIRMNR = FIRM.NR");
            if (!DataHelper.IsDataExists(dt))
            {
                XtraMessageBox.Show("Logoda Hiçbir Şirket Bulunamadı", "Hatalı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            gridControl1.DataSource = dt;
            GridViewDesigner.CustomizeGrid(gridView1);
        }
    }
}
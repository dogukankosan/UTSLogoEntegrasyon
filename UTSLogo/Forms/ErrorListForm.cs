using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using UTSLogo.Classes;

namespace UTSLogo.Forms
{
    public partial class ErrorListForm : XtraForm
    {
        private string _userName = "";
        public ErrorListForm(string userName = "")
        {
            InitializeComponent();
            _userName = userName;
        }
        private async void ErrorListForm_Load(object sender, EventArgs e)
        {
            await LoadErrorLogsAsync();
        }
        private async Task LoadErrorLogsAsync()
        {
            try
            {
                string query;
                Dictionary<string, object> param = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(_userName))
                {
                    query = "SELECT ID, UserName, Details, Date_ FROM ErrorLogs WHERE UserName=@UserName ORDER BY Date_ DESC";
                    param.Add("@UserName", _userName);
                }
                else
                    query = "SELECT ID, UserName, Details, Date_ FROM ErrorLogs ORDER BY Date_ DESC";
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query, param);
                if (dt.Columns.Contains("UserName")) dt.Columns["UserName"].ColumnName = "Kullanıcı";
                if (dt.Columns.Contains("Details")) dt.Columns["Details"].ColumnName = "Açıklama";
                if (dt.Columns.Contains("Date_")) dt.Columns["Date_"].ColumnName = "Tarih";
                gridControl1.DataSource = dt;
                gridView1.BestFitColumns();
                GridViewDesigner.CustomizeGrid(gridView1); 
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Hata logları yüklenemedi:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridControl1.DataSource == null) return;
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Filter = "Excel Dosyası|*.xlsx",
                Title = "Excel Olarak Kaydet",
                FileName = "Hatalar.xlsx"
            };
            if (saveFile.ShowDialog() != DialogResult.OK) return;
            string filePath = saveFile.FileName;
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                        {
                            // Dosya açılabiliyorsa kapatıyoruz
                        }
                    }
                    catch
                    {
                        XtraMessageBox.Show("Dosya başka bir program tarafından kullanılıyor, kaydedilemiyor. Lütfen dosyayı kapatın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                gridControl1.ExportToXlsx(filePath);
                XtraMessageBox.Show("Excel dosyası başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Excel dosyası kaydedilemedi:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("Tüm hata loglarını silmek istediğinizden emin misiniz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    string query;
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    if (!string.IsNullOrWhiteSpace(_userName))
                    {
                        query = "DELETE FROM ErrorLogs WHERE UserName=@UserName";
                        param.Add("@UserName", _userName);
                    }
                    else
                        query = "DELETE FROM ErrorLogs";
                    await SQLiteCrud.InsertUpdateDeleteAsync(query, param);
                    await LoadErrorLogsAsync();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Hata logları silinemedi:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
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
        public UTSForm()
        {
            InitializeComponent();
        }
        private async void UTSForm_Load(object sender, EventArgs e)
        {
            await LoadUTSAsync();
        }
        private async Task LoadUTSAsync()
        {
            try
            {
                string query = "SELECT * FROM UTS LIMIT 1";
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query);
                if (dt != null && dt.Rows.Count > 0)
                    txt_Token.Text = dt.Rows[0]["Token"].ToString();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Veri yüklenirken hata oluştu:\n" + ex.Message);
            }
        }
        private async Task SaveUTSAsync()
        {
            string token = txt_Token.Text.Trim();
            if (string.IsNullOrWhiteSpace(token))
            {
                XtraMessageBox.Show("Token boş olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string checkQuery = "SELECT COUNT(*) FROM UTS";
            DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(checkQuery);
            bool exists = dt != null && dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0][0]) > 0;
            string query;
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@token", token }
            };
            if (exists)
                query = "UPDATE UTS SET Token = @token WHERE ID = 1";
            else
                query = "INSERT INTO UTS (Token) VALUES (@token)";
            var result = await SQLiteCrud.InsertUpdateDeleteAsync(query, parameters);
            if (result.Success)
            {
                XtraMessageBox.Show("Token başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
                XtraMessageBox.Show("Hata oluştu: " + result.ErrorMessage, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private async void btn_Save_Click(object sender, EventArgs e)
        {
            await SaveUTSAsync();
        }
    }
}
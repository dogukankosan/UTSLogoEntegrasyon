using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using UTSLogo.Classes;
using UTSLogo.Bussines;

namespace UTSLogo.Forms
{
    public partial class UsersForm : XtraForm
    {
        private int selectedID = 0;
        public UsersForm()
        {
            InitializeComponent();
        }
        private async void UsersForm_Load(object sender, EventArgs e)
        {
            txt_userName.Focus();
            btn_Eyes.Visible = false;
            await LoadUsersAsync();
            GridViewDesigner.CustomizeGrid(gridView1);
            gridView1.DoubleClick += GridView1_DoubleClick;
        }
        private async Task LoadUsersAsync()
        {
            string query = @"SELECT 
                                ID,
                                UserName,
                                CASE WHEN IsAdmin = 1 THEN 'Admin' ELSE 'Kullanıcı' END AS Yetki,
                                Status
                             FROM Users
                             ORDER BY ID DESC";
            DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query);
            gridControl1.DataSource = dt;
            if (gridView1.Columns["ID"] != null)
                gridView1.Columns["ID"].Visible = false;
            if (gridView1.Columns["UserName"] != null)
                gridView1.Columns["UserName"].Caption = "Kullanıcı Adı";
            if (gridView1.Columns["Yetki"] != null)
                gridView1.Columns["Yetki"].Caption = "Yetki";
            if (gridView1.Columns["Status"] != null)
            {
                gridView1.Columns["Status"].Caption = "Durum";
                gridView1.Columns["Status"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gridView1.RowCellStyle += gridView1_RowCellStyle;
                gridView1.CustomColumnDisplayText += gridView1_CustomColumnDisplayText; 
            }
        }
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "Status")
            {
                GridView view = sender as GridView;
                object statusValue = view.GetRowCellValue(e.RowHandle, "Status");
                if (statusValue != null)
                {
                    int status = Convert.ToInt32(statusValue);
                    if (status == 0) 
                    {
                        e.Appearance.BackColor = Color.LightGreen;
                        e.Appearance.ForeColor = Color.Black;
                    }
                    else
                    {
                        e.Appearance.BackColor = Color.LightCoral;
                        e.Appearance.ForeColor = Color.Black;
                    }
                }
            }
        }
        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Status" && e.Value != null)
            {
                int status = Convert.ToInt32(e.Value);
                e.DisplayText = status == 0 ? "Aktif" : "Pasif";
            }
        }
        private async void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            await LoadSelectedRowAsync();
        }
        private async Task LoadSelectedRowAsync()
        {
            int rowHandle = gridView1.FocusedRowHandle;
            if (rowHandle < 0) return;
            object idValue = gridView1.GetRowCellValue(rowHandle, "ID");
            object userValue = gridView1.GetRowCellValue(rowHandle, "UserName");
            object adminValue = gridView1.GetRowCellValue(rowHandle, "Yetki");
            object statusValue = gridView1.GetRowCellValue(rowHandle, "Status");
            if (idValue == null || userValue == null)
                return;
            selectedID = Convert.ToInt32(idValue);
            txt_userName.Text = userValue.ToString();
            chc_IsAdmin.Checked = adminValue != null && adminValue.ToString() == "Admin";
            chk_Status.Checked = statusValue != null && Convert.ToInt32(statusValue) == 0;
            await LoadPasswordByID(selectedID);
        }
        private async Task LoadPasswordByID(int id)
        {
            string q = "SELECT Password FROM Users WHERE ID=@id";
            Dictionary<string, object> param = new Dictionary<string, object>() { { "@id", id } };
            DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(q, param);
            txt_Password.Text = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                string encrypted = dt.Rows[0]["Password"].ToString();
                string decrypted = await EncryptionHelper.Decrypt(encrypted);
                txt_Password.Text = decrypted;
            }
        }
        private async void btn_Save_Click(object sender, EventArgs e)
        {
            if (!await UserValidator.ValidateAsync(txt_userName, txt_Password, selectedID))
                return;
            string encryptedPassword = await EncryptionHelper.Encrypt(txt_Password.Text.Trim());
            string query;
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "@user", txt_userName.Text.Trim() },
                { "@pass", encryptedPassword },
                { "@admin", chc_IsAdmin.Checked ? 1 : 0 },
                { "@firma", "" },
                { "@period", "" },
                { "@thema", "" },
                { "@status", chk_Status.Checked ? 0 : 1 }
            };
            if (selectedID == 0)
                query = @"INSERT INTO Users (UserName, Password, IsAdmin, FirmaNR, PeriodNR, Thema, Status) 
                          VALUES (@user, @pass, @admin, @firma, @period, @thema, @status)";
            else
            {
                query = @"UPDATE Users SET UserName=@user, Password=@pass, IsAdmin=@admin,
                          FirmaNR=@firma, PeriodNR=@period, Thema=@thema, Status=@status
                          WHERE ID=@id";
                param.Add("@id", selectedID);
            }
            var result = await SQLiteCrud.InsertUpdateDeleteAsync(query, param);
            if (result.Success)
            {
                XtraMessageBox.Show("Kullanıcı kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                await LoadUsersAsync();
            }
            else
                XtraMessageBox.Show("Hata: " + result.ErrorMessage, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void btn_Eyes_Click(object sender, EventArgs e)
        {
            txt_Password.Properties.PasswordChar = '\0';
            btn_Eyes.Visible = false;
            btn_NotEye.Visible = true;
            txt_Password.Focus();
        }
        private void btn_NotEye_Click(object sender, EventArgs e)
        {
            txt_Password.Properties.PasswordChar = '*';
            btn_Eyes.Visible = true;
            btn_NotEye.Visible = false;
            txt_Password.Focus();
        }
        private async void GridView1_DoubleClick(object sender, EventArgs e)
        {
            await LoadSelectedRowAsync();
        }
        private void ClearForm()
        {
            txt_userName.Text = "";
            txt_Password.Text = "";
            chc_IsAdmin.Checked = false;
            chk_Status.Checked = true; 
            selectedID = 0;
        }
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}
using DevExpress.XtraEditors;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using UTSLogo.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UTSLogo.Bussines
{
    internal class UserValidator
    {
        internal static async Task<bool> ValidateAsync(TextEdit userName, TextEdit password, int currentID)
        {
            #region UserName
            if (string.IsNullOrWhiteSpace(userName.Text))
            {
                XtraMessageBox.Show("Kullanıcı adı boş geçilemez", "Hatalı Giriş",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                userName.Focus();
                return false;
            }
            else if (userName.Text.Length > 100)
            {
                XtraMessageBox.Show("Kullanıcı adı 100 karakterden uzun olamaz", "Hatalı Giriş",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                userName.Focus();
                return false;
            }
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE UserName=@u AND ID<>@id";
            DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(checkQuery,
                new Dictionary<string, object>()
                {
                    { "@u", userName.Text },
                    { "@id", currentID }
                });
            if (dt != null && Convert.ToInt32(dt.Rows[0][0]) > 0)
            {
                XtraMessageBox.Show("Bu kullanıcı adı zaten mevcut!", "Hatalı Giriş",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                userName.Focus();
                return false;
            }
            #endregion

            #region Password
            if (string.IsNullOrWhiteSpace(password.Text))
            {
                XtraMessageBox.Show("Şifre boş geçilemez", "Hatalı Giriş",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                password.Focus();
                return false;
            }
            else if (password.Text.Length > 100)
            {
                XtraMessageBox.Show("Şifre 100 karakterden uzun olamaz", "Hatalı Giriş",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                password.Focus();
                return false;
            }
            #endregion
            return true;
        }
    }
}
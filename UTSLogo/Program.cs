using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using UTSLogo.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Localization;
using UTSLogo.Classes;

namespace UTSLogo
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                GridLocalizer.Active = new MyGridLocalizer();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                _ = TextLog.LogToSQLiteAsync("UYGULAMA BAŞLANGICI", "Uygulama başlangıcında hata: " + ex.Message);
                XtraMessageBox.Show("Uygulama başlatılamadı. Detaylar log dosyasına yazıldı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
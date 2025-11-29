using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DevExpress.XtraEditors;
using System.Data;
using System.Windows.Forms;
using UTSLogo.Models; // Modelleri dahil ediyoruz

namespace UTSLogo.Classes
{
    public static class UTSApiClient
    {
        private const string BaseUrl = "https://utsuygulama.saglik.gov.tr/UTS/uh/rest/";
        private const string SorgulamaEndpoint = "tekilUrun/sorgula";

        // --- Token Çekme Metodu ---
        private static async Task<string> GetUtsTokenAsync()
        {
            const string query = "SELECT Token FROM UTS LIMIT 1";
            try
            {
                DataTable result = await SQLiteCrud.GetDataFromSQLiteAsync(query);

                if (result != null && result.Rows.Count > 0 && result.Columns.Contains("Token"))
                {
                    return result.Rows[0]["Token"].ToString();
                }

                await TextLog.LogToSQLiteAsync("API Token Hatası", "UTS tablosunda token bulunamadı veya tablo boş.");
                XtraMessageBox.Show("UTS Token veritabanında bulunamadı. Lütfen ayarları kontrol edin.", "Token Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("API Token Hatası", $"Token çekilirken hata oluştu: {ex.Message}");
                XtraMessageBox.Show($"Token çekilirken beklenmedik bir hata oluştu: {ex.Message}", "Token Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // --- Ana Sorgulama Metodu ---
        public static async Task<UtsTekilUrunYanit> SorgulaTekilUrunAsync(string uno, string lno, string san, string userName)
        {
            string utsToken = await GetUtsTokenAsync();
            if (string.IsNullOrEmpty(utsToken))
            {
                return null;
            }

            var requestData = new TekilUrunSorguRequest
            {
                Uno = uno,
                Lno  = lno,
                San = san
            };

            string jsonContent = JsonConvert.SerializeObject(requestData);
            string fullUrl = $"{BaseUrl}{SorgulamaEndpoint}";

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("utsToken", utsToken);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(fullUrl, httpContent);
                    string responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = JsonConvert.DeserializeObject<UtsTekilUrunYanit>(responseString);
                        await TextLog.LogToSQLiteAsync(userName, $"UTS Sorgu Başarılı: UNO={uno}, Yanıt Kodu: {response.StatusCode}");
                        return apiResponse;
                    }
                    else
                    {
                        string logMessage = $"UTS API Hatası - HTTP Kodu: {response.StatusCode} | Detay: {responseString} | UNO={uno}";
                        await TextLog.LogToSQLiteAsync(userName, logMessage);
                        XtraMessageBox.Show($"UTS'den Hata Alındı ({response.StatusCode}). Detaylar loglandı.", "API Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return new UtsTekilUrunYanit { UrunListesi = null, Mesaj = $"HTTP Hatası: {response.StatusCode} - {responseString.Substring(0, Math.Min(responseString.Length, 100))}..." };
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    string logMessage = $"UTS Sorgu sırasında Ağ Hatası: {httpEx.Message} | UNO={uno}";
                    await TextLog.LogToSQLiteAsync(userName, logMessage);
                    XtraMessageBox.Show($"UTS sorgusu sırasında ağ hatası oluştu: {httpEx.Message}", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                catch (Exception ex)
                {
                    string logMessage = $"UTS Sorgu sırasında Genel Hata: {ex.Message} | UNO={uno}";
                    await TextLog.LogToSQLiteAsync(userName, logMessage);
                    XtraMessageBox.Show($"UTS sorgusu sırasında beklenmedik bir hata oluştu: {ex.Message}", "Genel Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }
    }
}
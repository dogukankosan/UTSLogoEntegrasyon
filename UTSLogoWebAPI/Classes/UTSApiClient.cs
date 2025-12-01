using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UTSLogoWebAPI.Models;

namespace UTSLogoWebAPI.Classes
{
    public static class UTSApiClient
    {
        private const string BaseUrl = "https://utsuygulama.saglik.gov.tr/UTS/uh/rest/";
        private const string SorgulamaEndpoint = "tekilUrun/sorgula";

        public static async Task<(UtsTekilUrunYanit result, bool success)>
            SorgulaTekilUrunAsync(string uno, string lno, string sn, string utsToken, string customerName)
        {
            if (string.IsNullOrWhiteSpace(utsToken))
            {
                await SQLCrud.LogErrorAsync($"[UTS] Token boş. Müşteri: {customerName}");
                return (new UtsTekilUrunYanit { Mesaj = "UTS Token tanımsız." }, false);
            }

            var requestData = new TekilUrunSorguRequest
            {
                Uno = uno,
                Lno = lno,
                San = sn
            };

            string jsonContent;
            try
            {
                jsonContent = JsonConvert.SerializeObject(requestData);
            }
            catch (Exception ex)
            {
                await SQLCrud.LogErrorAsync(
                    $"[UTS] JSON serileştirme hatası: {ex.Message} | Müşteri={customerName} | UNO={uno}");
                return (new UtsTekilUrunYanit { Mesaj = "İstek oluşturulamadı." }, false);
            }

            string fullUrl = $"{BaseUrl}{SorgulamaEndpoint}";

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();

                    // ⚠️ SADECE utsToken - Sağlık Bakanlığı bunu istiyor
                    client.DefaultRequestHeaders.Add("utsToken", utsToken);

                    // ❌ X-Api-Key BURADA OLMAMALI - Bu senin API'nin header'ı, UTS'nin değil!

                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(fullUrl, httpContent);
                    string responseString = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        await SQLCrud.LogErrorAsync(
                            $"[UTS] HTTP Hatası | Müşteri={customerName} | UNO={uno} | Kod={response.StatusCode} | Cevap={responseString}");
                        return (new UtsTekilUrunYanit { Mesaj = $"UTS Hata: {response.StatusCode}" }, false);
                    }

                    try
                    {
                        var apiResponse = JsonConvert.DeserializeObject<UtsTekilUrunYanit>(responseString);
                        return (apiResponse, true);
                    }
                    catch (Exception jsonEx)
                    {
                        await SQLCrud.LogErrorAsync(
                            $"[UTS] JSON parse hatası | Müşteri={customerName} | Hata={jsonEx.Message}");
                        return (new UtsTekilUrunYanit { Mesaj = "UTS cevabı işlenemedi." }, false);
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    await SQLCrud.LogErrorAsync(
                        $"[UTS] Bağlantı hatası | Müşteri={customerName} | Hata={httpEx.Message}");
                    return (new UtsTekilUrunYanit { Mesaj = "UTS bağlantı hatası." }, false);
                }
                catch (Exception ex)
                {
                    await SQLCrud.LogErrorAsync(
                        $"[UTS] Genel hata | Müşteri={customerName} | Hata={ex.Message}");
                    return (new UtsTekilUrunYanit { Mesaj = "Beklenmeyen hata oluştu." }, false);
                }
            }
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UTSLogo.Models;

namespace UTSLogo.Classes
{
    internal class UTSApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string BaseUrl = $"{ApiConstants.BaseApiUrl}UTS/";
        private static bool _headerSet = false;

        #region ==================== UTS SORGULAMA ====================
        internal static async Task<UTSQueryResponse> QueryAsync(string uno, string lno, string sn, int adet)
        {
            try
            {
                await EnsureHeaderSetAsync();
                string customerGUID = await GetCustomerGUIDAsync();
                if (string.IsNullOrEmpty(customerGUID))
                {
                    return new UTSQueryResponse
                    {
                        Success = false,
                        Message = "Müşteri kaydı bulunamadı. Lütfen önce kayıt olun."
                    };
                }
                UTSQueryRequest request = new UTSQueryRequest
                {
                    CustomerGUID = customerGUID,
                    UNO = uno,
                    LNO = lno,
                    SN = sn,
                    Adet = adet
                };
                string json = JsonConvert.SerializeObject(request);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}Query", content);
                string responseJson = await response.Content.ReadAsStringAsync();
                await TextLog.LogToSQLiteAsync("UTSApiClient", $"API Yanıt: {responseJson}");
                UTSQueryResponse result = JsonConvert.DeserializeObject<UTSQueryResponse>(responseJson);
                if (result == null)
                {
                    await TextLog.LogToSQLiteAsync("UTSApiClient", $"Deserialize null döndü! Raw: {responseJson}");
                    return new UTSQueryResponse
                    {
                        Success = false,
                        Message = "API yanıtı işlenemedi."
                    };
                }
                return result;
            }
            catch (TaskCanceledException)
            {
                await TextLog.LogToSQLiteAsync("UTSApiClient", $"Timeout hatası");
                return new UTSQueryResponse
                {
                    Success = false,
                    Message = "Sunucu yanıt vermedi (Timeout)."
                };
            }
            catch (HttpRequestException ex)
            {
                await TextLog.LogToSQLiteAsync("UTSApiClient", $"HTTP hatası: {ex.Message}");
                return new UTSQueryResponse
                {
                    Success = false,
                    Message = "Sunucuya bağlanılamadı."
                };
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("UTSApiClient", $"Genel hata: {ex.Message}");
                return new UTSQueryResponse
                {
                    Success = false,
                    Message = $"Hata: {ex.Message}"
                };
            }
        }
        #endregion

        #region ==================== HELPER METODLAR ====================
        private static async Task EnsureHeaderSetAsync()
        {
            if (_headerSet) return;
            try
            {
                string query = "SELECT CustomerToken FROM ClientSettings LIMIT 1";
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query);
                if (dt?.Rows.Count > 0)
                {
                    string apiKey = dt.Rows[0]["CustomerToken"]?.ToString();
                    if (!string.IsNullOrEmpty(apiKey))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("X-Api-Key");
                        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
                        _headerSet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("UTSApiClient", $"Header ayarlama hatası: {ex.Message}");
            }
        }
        private static async Task<string> GetCustomerGUIDAsync()
        {
            try
            {
                string query = "SELECT CustomerGUID FROM ClientSettings LIMIT 1";
                DataTable dt = await SQLiteCrud.GetDataFromSQLiteAsync(query);
                if (dt?.Rows.Count > 0)
                    return dt.Rows[0]["CustomerGUID"]?.ToString();
                return null;
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("UTSApiClient", $"GetCustomerGUIDAsync hata: {ex.Message}");
                return null;
            }
        }
        #endregion

    }
}
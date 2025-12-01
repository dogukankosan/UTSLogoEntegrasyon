using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using UTSLogo.Models;

namespace UTSLogo.Classes
{
    public static class WebAPIClient
    {
        private static readonly HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
        #region ==================== GENERIC HELPER ====================
        private static async Task<string> GetEncryptedTokenAsync(string apiToken)
        {
            try
            {
                return await EncryptionHelper.Encrypt(apiToken);
            }
            catch
            {
                return apiToken; 
            }
        }
        private static async Task LogErrorAsync(string userName, string details)
        {
            await TextLog.LogToSQLiteAsync(userName, details);
        }
        private static async Task<(bool success, JObject json, string message)> SendRequestAsync(
            string url, HttpMethod method, object requestData, string apiToken, string userName)
        {
            try
            {
                string encryptedToken = await GetEncryptedTokenAsync(apiToken);
                HttpRequestMessage request = new HttpRequestMessage(method, url);
                request.Headers.Add("X-Api-Key", encryptedToken);
                if (requestData != null)
                {
                    string jsonContent = JsonConvert.SerializeObject(requestData);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }
                HttpResponseMessage response = await client.SendAsync(request);
                string responseStr = await response.Content.ReadAsStringAsync();
                JObject json = null;
                try { json = JObject.Parse(responseStr); } catch {  }
                if (response.IsSuccessStatusCode)
                    return (true, json, "Başarılı");
                string msg = json?["message"]?.Value<string>() ?? responseStr;
                await LogErrorAsync(userName, $"API Hatası ({(int)response.StatusCode}): {msg}");
                return (false, json, msg);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(userName, $"Bağlantı Hatası: {ex.Message}");
                return (false, null, $"Bağlantı Hatası: {ex.Message}");
            }
        }
        #endregion

        #region ==================== MÜŞTERİ KAYIT ====================
        public static async Task<(bool success, string customerGUID, string message)> RegisterCustomerAsync(
            string customerName, string utsToken, int count, string apiToken)
        {
            string url = $"{ApiConstants.BaseApiUrl}Customers/Register";
            var requestData = new { CustomerName = customerName, UTSToken = utsToken, Count = count };
            var (success, json, message) = await SendRequestAsync(url, HttpMethod.Post, requestData, apiToken, customerName);
            if (success)
            {
                string guid = json?["customerGUID"]?.Value<string>() ?? "";
                return (true, guid, message);
            }
            return (false, null, message);
        }
        #endregion

        #region ==================== MÜŞTERİ BİLGİ ====================
        public static async Task<(bool success, string customerName, int count, string utsToken, string message)>
            GetCustomerInfoAsync(string customerGUID, string apiToken, string userName)
        {
            string url = $"{ApiConstants.BaseApiUrl}Customers/Info?guid={Uri.EscapeDataString(customerGUID)}";
            var (success, json, message) = await SendRequestAsync(url, HttpMethod.Get, null, apiToken, userName);
            if (success)
            {
                string name = json?["customerName"]?.Value<string>() ?? "";
                int count = json?["count"]?.Value<int>() ?? 0;
                string utsToken = json?["utsToken"]?.Value<string>() ?? "";
                return (true, name, count, utsToken, message);
            }
            return (false, null, 0, null, message);
        }
        public static async Task<(bool success, int count, string message)>
            GetCustomerCountAsync(string customerGUID, string apiToken, string userName)
        {
            string url = $"{ApiConstants.BaseApiUrl}Customers/Count?guid={Uri.EscapeDataString(customerGUID)}";
            var (success, json, message) = await SendRequestAsync(url, HttpMethod.Get, null, apiToken, userName);
            if (success)
            {
                int count = 0;
                if (json != null && int.TryParse(json["count"]?.ToString(), out count))
                    return (true, count, message);
                return (false, 0, "Kontör bilgisi sayı formatında değil.");
            }
            return (false, 0, message);
        }
        #endregion

        #region ==================== KONTÖR İŞLEMLERİ ====================
        public static async Task<(bool success, int oldCount, int newCount, string message)>
            DeductCountAsync(string customerGUID, int amount, string apiToken, string userName)
        {
            string url = $"{ApiConstants.BaseApiUrl}Customers/DeductCount";
            var requestData = new { CustomerGUID = customerGUID, Amount = amount };
            var (success, json, message) = await SendRequestAsync(url, HttpMethod.Put, requestData, apiToken, userName);
            if (success)
            {
                int oldCount = json?["oldCount"]?.Value<int>() ?? 0;
                int newCount = json?["newCount"]?.Value<int>() ?? 0;
                return (true, oldCount, newCount, message);
            }
            return (false, 0, 0, message);
        }

        public static async Task<(bool success, int oldCount, int newCount, string message)>
            AddCountAsync(string customerGUID, int amount, string apiToken, string userName)
        {
            string url = $"{ApiConstants.BaseApiUrl}Customers/AddCount";
            var requestData = new { CustomerGUID = customerGUID, Amount = amount };
            var (success, json, message) = await SendRequestAsync(url, HttpMethod.Put, requestData, apiToken, userName);
            if (success)
            {
                int oldCount = json?["oldCount"]?.Value<int>() ?? 0;
                int newCount = json?["newCount"]?.Value<int>() ?? 0;
                return (true, oldCount, newCount, message);
            }
         
            return (false, 0, 0, message);
        }
        #endregion

        #region ==================== UTS TOKEN ====================
        public static async Task<(bool success, string message)>
            UpdateUTSTokenAsync(string customerGUID, string utsToken, string apiToken, string userName)
        {
            string url = $"{ApiConstants.BaseApiUrl}Customers/UpdateUTSToken";
            var requestData = new { CustomerGUID = customerGUID, UTSToken = utsToken };
            var (success, json, message) = await SendRequestAsync(url, HttpMethod.Put, requestData, apiToken, userName);
            return (success, message);
        }
        #endregion
    }
}
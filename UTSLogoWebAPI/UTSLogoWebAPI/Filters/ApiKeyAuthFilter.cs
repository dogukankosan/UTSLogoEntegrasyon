using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using UTSLogoWebAPI.Classes;
using System;
using System.Threading.Tasks;

namespace UTSLogoWebAPI.Filters
{
    public class ApiKeyAuthFilter : IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        private readonly IConfiguration _configuration;
        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string configurationApiKey = _configuration["ApiSettings:CustomerApiKey"];
            if (string.IsNullOrEmpty(configurationApiKey))
            {
                await SQLCrud.LogErrorAsync("[ApiKeyAuthFilter] ApiSettings:CustomerApiKey eksik!");
                context.Result = new StatusCodeResult(500);
                return;
            }
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var sentApiKey))
            {
                await SQLCrud.LogErrorAsync("[ApiKeyAuthFilter] X-Api-Key header eksik.");
                context.Result = new UnauthorizedResult();
                return;
            }
            string receivedToken = sentApiKey.ToString();
            string decryptedToken = null;
            try
            {
                decryptedToken = await EncryptionHelper.Decrypt(receivedToken);
            }
            catch
            {
                decryptedToken = null;
            }
            bool isValid =
                   (decryptedToken != null && decryptedToken == configurationApiKey)
                || (receivedToken == configurationApiKey);
            if (!isValid)
            {
                await SQLCrud.LogErrorAsync("[ApiKeyAuthFilter] Yetkilendirme başarısız. Token eşleşmedi.");
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
        }
    }
}
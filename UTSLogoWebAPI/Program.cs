using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.AspNetCore.Mvc;
using UTSLogoWebAPI.Filters;

namespace UTSLogoWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://0.0.0.0:8898");
            builder.Host.UseWindowsService();
            builder.Services.AddSingleton<ApiKeyAuthFilter>();
            builder.Services.AddSingleton<GlobalExceptionMiddleware>();
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new ServiceFilterAttribute(typeof(ApiKeyAuthFilter)));
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            WebApplication app = builder.Build();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
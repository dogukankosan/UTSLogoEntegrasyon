using Microsoft.AspNetCore.Http;
using System.Net;
using UTSLogoWebAPI.Classes;

public class GlobalExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await SQLCrud.LogErrorAsync(
                $"[GLOBAL EXCEPTION] Path: {context.Request.Path} | Error: {ex.Message}\n{ex.StackTrace}"
            );
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"success\": false, \"message\": \"Beklenmeyen bir hata oluştu. (Global Handler)\"}"
            );
        }
    }
}
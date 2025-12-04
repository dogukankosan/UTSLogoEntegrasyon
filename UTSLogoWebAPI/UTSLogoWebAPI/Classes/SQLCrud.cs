using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace UTSLogoWebAPI.Classes
{
    public class SQLCrud
    {
        private static string _connectionString = null;
        private static string GetConnectionString()
        {
            if (_connectionString != null)
                return _connectionString;
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                _connectionString = configuration.GetConnectionString("DefaultConnection");
                return _connectionString;
            }
            catch (Exception ex)
            {
                LogToFile($"[GetConnectionString HATA] {ex.Message}");
                return null;
            }
        }
        private static void LogToFile(string message)
        {
            try
            {
                string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                string logFile = Path.Combine(logFolder, $"log_{DateTime.Now:yyyy-MM-dd}.txt");
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                File.AppendAllText(logFile, logEntry);
            }
            catch
            {
            }
        }
        internal static async Task LogErrorAsync(string details, short customerId = 0)
        {
            string connectionString = GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
                return;
            try
            {
                string query = "INSERT INTO dbo.ErrorLogs (CustomerID, Details) VALUES (@CustomerID, @Details)";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                        cmd.Parameters.AddWithValue("@Details", details ?? (object)DBNull.Value);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LogToFile($"[DB LOG YAZMA HATASI] {ex.Message}");
            }
        }
        internal static async Task<bool> ExecuteCrudAsync(string query, Dictionary<string, object> parameters = null)
        {
            string connectionString = GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                await LogErrorAsync("Connection string okunamadı.");
                return false;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                        int result = await cmd.ExecuteNonQueryAsync();
                        return result > 0;  
                    }
                }
            }
            catch (Exception ex)
            {
                await LogErrorAsync(
                    $"[ExecuteCrudAsync HATA] {ex.Message}\nQuery:{query}"
                );
                return false;
            }
        }
        internal static async Task<object> ExecuteScalarAsync(string query, Dictionary<string, object> parameters = null)
        {
            string connectionString = GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                await LogErrorAsync("Connection string okunamadı.");
                return null;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }

                        return await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await LogErrorAsync(
                    $"[ExecuteScalarAsync HATA] {ex.Message}\nQuery:{query}"
                );
                return null;
            }
        }
        internal static async Task<DataTable> GetDataTableAsync(string query, Dictionary<string, object> parameters = null)
        {
            string connectionString = GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                await LogErrorAsync("Connection string okunamadı.");
                return null;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await LogErrorAsync(
                    $"[GetDataTableAsync HATA] {ex.Message}\nQuery:{query}"
                );
                return null;
            }
        }
    }
}

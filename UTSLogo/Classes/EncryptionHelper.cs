using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UTSLogo.Classes
{
    internal static class EncryptionHelper
    {

        private static readonly byte[] keyBytes = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        private static readonly byte[] ivBytes = Encoding.UTF8.GetBytes("1234567890123456");
        internal static async Task<string> Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                return null;
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = ivBytes;
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                await swEncrypt.WriteAsync(plainText);
                                await swEncrypt.FlushAsync();
                                csEncrypt.FlushFinalBlock();
                            }
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("Encrypt", $"Şifreleme hatası: {ex.Message}");
                return null;
            }
        }
        internal static async Task<string> Decrypt(string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return null;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = ivBytes;
                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return await srDecrypt.ReadToEndAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await TextLog.LogToSQLiteAsync("Decrypt", $"Şifre çözme hatası: {ex.Message}");
                return null;
            }
        }
    }
}

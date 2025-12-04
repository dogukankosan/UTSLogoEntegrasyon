using System.Security.Cryptography;
using System.Text;

namespace UTSLogoWebAPI.Classes
{
    public class EncryptionHelper
    {
        private static readonly byte[] keyBytes = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        private static readonly byte[] ivBytes = Encoding.UTF8.GetBytes("1234567890123456");
        internal static async Task<string> Decrypt(string cipherText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText))
                {
                    await SQLCrud.LogErrorAsync("[EncryptionHelper.Decrypt] cipherText boş veya null.", 0);
                    return null;
                }
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = ivBytes;
                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        string decrypted = srDecrypt.ReadToEnd();
                        return decrypted;
                    }
                }
            }
            catch (FormatException ex)
            {
                await SQLCrud.LogErrorAsync($"[EncryptionHelper.Decrypt] Base64 format hatası: {ex.Message} | Input: {cipherText?.Substring(0, Math.Min(50, cipherText?.Length ?? 0))}...", 0);
                return null;
            }
            catch (CryptographicException ex)
            {
                await SQLCrud.LogErrorAsync($"[EncryptionHelper.Decrypt] Kriptografik hata: {ex.Message}", 0);
                return null;
            }
            catch (Exception ex)
            {
                await SQLCrud.LogErrorAsync($"[EncryptionHelper.Decrypt] Genel hata: {ex.Message} | StackTrace: {ex.StackTrace}", 0);
                return null;
            }
        }
    }
}
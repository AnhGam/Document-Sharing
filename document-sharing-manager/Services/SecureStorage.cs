using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace document_sharing_manager.Services
{
    public static class SecureStorage
    {
        private static readonly string StoragePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DocShare",
            "auth.dat");

        public static void SaveTokens(string accessToken, string refreshToken)
        {
            try
            {
                string data = $"{accessToken}|{refreshToken}";
                byte[] plainBytes = Encoding.UTF8.GetBytes(data);
                byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
                
                string dir = Path.GetDirectoryName(StoragePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                
                File.WriteAllBytes(StoragePath, encryptedBytes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error saving tokens: " + ex.Message);
            }
        }

        public static (string accessToken, string refreshToken) LoadTokens()
        {
            try
            {
                if (!File.Exists(StoragePath)) return (null, null);

                byte[] encryptedBytes = File.ReadAllBytes(StoragePath);
                byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
                string data = Encoding.UTF8.GetString(plainBytes);

                string[] parts = data.Split('|');
                if (parts.Length == 2)
                {
                    return (parts[0], parts[1]);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading tokens: " + ex.Message);
            }
            return (null, null);
        }

        public static void ClearTokens()
        {
            try
            {
                if (File.Exists(StoragePath)) File.Delete(StoragePath);
            }
            catch { }
        }
    }
}

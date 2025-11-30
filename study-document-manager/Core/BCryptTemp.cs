using System;
using System.Security.Cryptography;
using System.Text;

namespace BCrypt.Net
{
    /// <summary>
    /// Temporary BCrypt implementation (simplified)
    /// Install BCrypt.Net-Next package để thay thế class này
    /// </summary>
    public static class BCrypt
    {
        /// <summary>
        /// Hash password
        /// </summary>
        public static string HashPassword(string password)
        {
            // Simplified hash - KHÔNG dùng trong production
            // Chỉ để compile được, phải install BCrypt.Net-Next
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt_study_manager"));
                StringBuilder builder = new StringBuilder();
                builder.Append("$2a$11$");
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString().Substring(0, 60);
            }
        }

        /// <summary>
        /// Verify password
        /// </summary>
        public static bool Verify(string password, string hash)
        {
            // Simplified verify - KHÔNG dùng trong production
            string newHash = HashPassword(password);
            return newHash == hash;
        }
    }
}

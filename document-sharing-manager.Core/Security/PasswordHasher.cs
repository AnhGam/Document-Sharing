using System;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace document_sharing_manager.Core.Security
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 600000;

        public static string HashPassword(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return $"{Iterations}.{salt}.{key}";
            }
        }

        public static bool VerifyPassword(string hash, string password)
        {
            if (string.IsNullOrEmpty(hash))
            {
                return false;
            }

            var parts = hash.Split(new[] { '.' }, 3);
            if (parts.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(parts[0], out var iterations) || iterations <= 0)
            {
                return false;
            }

            byte[] salt;
            byte[] key;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                key = Convert.FromBase64String(parts[2]);
            }
            catch
            {
                return false;
            }

            try
            {
                using (var algorithm = new Rfc2898DeriveBytes(password, salt, iterations))
                {
                    var keyToCheck = algorithm.GetBytes(KeySize);
                    return FixedTimeEquals(keyToCheck, key);
                }
            }
            catch
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
                return false;

            int difference = 0;
            for (int i = 0; i < left.Length; i++)
            {
                difference |= left[i] ^ right[i];
            }

            return difference == 0;
        }
    }
}

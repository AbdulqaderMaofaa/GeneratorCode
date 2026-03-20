using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace GeneratorCode.Core.Helpers
{
    public static class PasswordEncryption
    {
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
                    return Convert.ToBase64String(encryptedBytes);
                }

                return EncryptFallback(plainText);
            }
            catch
            {
                return EncryptFallback(plainText);
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var cipherBytes = Convert.FromBase64String(cipherText);
                    var decryptedBytes = ProtectedData.Unprotect(cipherBytes, null, DataProtectionScope.CurrentUser);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }

                return DecryptFallback(cipherText);
            }
            catch
            {
                return DecryptFallback(cipherText);
            }
        }

        private static string EncryptFallback(string plainText)
        {
            try
            {
                var key = DeriveKeyFromMachine();
                using var aes = Aes.Create();
                aes.Key = key;
                aes.GenerateIV();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor();
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                var result = new byte[aes.IV.Length + encryptedBytes.Length];
                Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
                Array.Copy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);
                return Convert.ToBase64String(result);
            }
            catch
            {
                return plainText;
            }
        }

        private static string DecryptFallback(string cipherText)
        {
            try
            {
                var key = DeriveKeyFromMachine();
                var fullCipher = Convert.FromBase64String(cipherText);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - 16];
                Array.Copy(fullCipher, 0, iv, 0, 16);
                Array.Copy(fullCipher, 16, cipher, 0, cipher.Length);
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor();
                var decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return cipherText;
            }
        }

        private static byte[] DeriveKeyFromMachine()
        {
            var machineName = Environment.MachineName ?? "DefaultMachine";
            var userName = Environment.UserName ?? "DefaultUser";
            var seed = Encoding.UTF8.GetBytes($"{machineName}:{userName}:GeneratorCode");
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(seed);
        }
    }
}

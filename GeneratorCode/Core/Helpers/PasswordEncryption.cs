using System;
using System.Security.Cryptography;
using System.Text;

namespace GeneratorCode.Core.Helpers
{
    /// <summary>
    /// فئة مساعدة لتشفير وفك تشفير كلمات المرور
    /// </summary>
    public static class PasswordEncryption
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("GeneratorCode2024!SecretKeyForEncryption12345678"); // 32 bytes
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("InitVector123456"); // 16 bytes

        /// <summary>
        /// تشفير كلمة المرور
        /// </summary>
        /// <param name="plainText">كلمة المرور النصية</param>
        /// <returns>كلمة المرور المشفرة (Base64)</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        var plainBytes = Encoding.UTF8.GetBytes(plainText);
                        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
            catch
            {
                // في حالة الفشل، إرجاع النص الأصلي (للتوافق مع الكود القديم)
                return plainText;
            }
        }

        /// <summary>
        /// فك تشفير كلمة المرور
        /// </summary>
        /// <param name="cipherText">كلمة المرور المشفرة (Base64)</param>
        /// <returns>كلمة المرور النصية</returns>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var cipherBytes = Convert.FromBase64String(cipherText);
                        var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch
            {
                // في حالة الفشل، إرجاع النص الأصلي (قد يكون غير مشفر)
                return cipherText;
            }
        }
    }
}


using System;
using System.IO;
using System.Security.Cryptography;

namespace SOQET.Security
{
    /// <summary>
    /// Uses AES to decrypt and encrypt save files
    /// </summary>
    public static class AESEncryption
    {
        /// <summary>
        /// encryption key
        /// </summary>
        private static readonly byte[] Key = new byte[32];

        /// <summary>
        /// initialization vector
        /// </summary>
        private static readonly byte[] IV = new byte[16];

        public static void EncryptFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                byte[] encryptedBytes;

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (FileStream inputStream = File.OpenRead(filePath))
                    using (MemoryStream memoryStream = new MemoryStream())
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        inputStream.CopyTo(cryptoStream);
                        cryptoStream.FlushFinalBlock();
                        encryptedBytes = memoryStream.ToArray();
                    }
                }

                File.WriteAllBytes(filePath, encryptedBytes);
            }
        }

        public static void DecryptFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                byte[] decryptedBytes;

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (FileStream inputStream = File.OpenRead(filePath))
                    using (MemoryStream memoryStream = new MemoryStream())
                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(memoryStream);
                        decryptedBytes = memoryStream.ToArray();
                    }
                }

                File.WriteAllBytes(filePath, decryptedBytes);
            }
        }
    }
}
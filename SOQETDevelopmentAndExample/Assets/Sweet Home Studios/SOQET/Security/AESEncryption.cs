using System;
using System.IO;
using System.Security.Cryptography;

namespace SOQET.Security
{
    public static class AESEncryption
    {
        private static readonly byte[] Key = new byte[32]; // 256-bit key
        private static readonly byte[] IV = new byte[16]; // 128-bit IV

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
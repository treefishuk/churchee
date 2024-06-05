using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Churchee.Common.Helpers
{
    public static class AESEncryptionHelper
    {
        public static string Encrypt(string key, string plainText)
        {
            ArgumentException.ThrowIfNullOrEmpty(plainText);
            ArgumentException.ThrowIfNullOrEmpty(key);

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.ECB;

                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using var msEncrypt = new MemoryStream();
                using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                encrypted = msEncrypt.ToArray();
            }

            // Return the encrypted result as a Base64 String.
            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string key, string dataToDecrypt)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentException.ThrowIfNullOrEmpty(dataToDecrypt);

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            byte[] cipherText = Convert.FromBase64String(dataToDecrypt);

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.ECB;

                // Create a decryptor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using var msDecrypt = new MemoryStream(cipherText);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }

            return plaintext;
        }
    }
}

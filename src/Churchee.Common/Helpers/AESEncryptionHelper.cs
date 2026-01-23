using System;
using System.Security.Cryptography;
using System.Text;

namespace Churchee.Common.Helpers
{
    public static class AesEncryptionHelper
    {
        public static string Encrypt(string key, string plainText)
        {
            ArgumentException.ThrowIfNullOrEmpty(plainText);
            ArgumentException.ThrowIfNullOrEmpty(key);

            using var aesAlg = new AesGcm(Encoding.UTF8.GetBytes(key), tagSizeInBytes: 16);
            byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            RandomNumberGenerator.Fill(nonce);

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[16]; // 16 bytes for the tag size

            aesAlg.Encrypt(nonce, plaintextBytes, ciphertext, tag);

            byte[] result = new byte[nonce.Length + ciphertext.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length + ciphertext.Length, tag.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string key, string cipherText)
        {
            ArgumentException.ThrowIfNullOrEmpty(cipherText);
            ArgumentException.ThrowIfNullOrEmpty(key);

            byte[] fullCipher = Convert.FromBase64String(cipherText);

            return DecryptAesGcm(key, fullCipher);
        }

        private static string DecryptAesGcm(string key, byte[] fullCipher)
        {
            using var aesAlg = new AesGcm(Encoding.UTF8.GetBytes(key), tagSizeInBytes: 16);
            byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            byte[] tag = new byte[16]; // 16 bytes for the tag size
            byte[] ciphertext = new byte[fullCipher.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(fullCipher, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(fullCipher, nonce.Length, ciphertext, 0, ciphertext.Length);
            Buffer.BlockCopy(fullCipher, nonce.Length + ciphertext.Length, tag, 0, tag.Length);

            byte[] plaintextBytes = new byte[ciphertext.Length];
            aesAlg.Decrypt(nonce, ciphertext, tag, plaintextBytes);

            return Encoding.UTF8.GetString(plaintextBytes);
        }
    }
}

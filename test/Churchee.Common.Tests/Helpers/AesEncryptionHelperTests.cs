using Churchee.Common.Helpers;

namespace Churchee.Common.Tests.Helpers
{
    public class AesEncryptionHelperTests
    {
        private const string Key = "aB3dE5fG7hI9jK1lM2nO4pQ6rS8tU0vW";

        [Fact]
        public void EncryptAndDecrypt_ShouldReturnOriginalText()
        {
            string originalText = "Hello, World!";
            string encryptedText = AesEncryptionHelper.Encrypt(Key, originalText);
            string decryptedText = AesEncryptionHelper.Decrypt(Key, encryptedText);

            Assert.Equal(originalText, decryptedText);
        }

        [Fact]
        public void LegacyEncryptAndDecrypt_ShouldReturnOriginalText()
        {
            string originalText = "Hello, World!";
            string legacyEncryptedText = "rPjYVUtojMvddQ7cDpjx9w==";
            string decryptedText = AesEncryptionHelper.Decrypt(Key, legacyEncryptedText);

            Assert.Equal(originalText, decryptedText);
        }

    }

}
using Churchee.Data.EntityFramework.Shared.Converters;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Data.EntityFramework.Admin.Tests.Converters
{
    public class EncryptionConvertorTests
    {
        private const string EncryptionKey = "0123456789abcdef";
        private readonly EncryptionConvertor _encryptionConvertor;

        public EncryptionConvertorTests()
        {
            _encryptionConvertor = new EncryptionConvertor(EncryptionKey);
        }

        [Fact]
        public void ConvertToProvider_ShouldEncryptValue()
        {
            // Arrange
            string plainText = "plain text";

            // Act
            string result = _encryptionConvertor.ConvertToProviderExpression.Compile().Invoke(plainText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().NotBe(plainText);
        }

        [Fact]
        public void ConvertFromProvider_ShouldDecryptValue()
        {
            // Arrange
            string encryptedText = "ngcvfFH0W2iqKypGqln0HGTHMmSgqcksFLjQAVuKKEuUX3YN+/Q=";
            string plainText = "plain text";

            // Act
            string result = _encryptionConvertor.ConvertFromProviderExpression.Compile().Invoke(encryptedText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().Be(plainText);
        }

        [Fact]
        public void ConvertToProvider_ShouldReturnEmptyString_WhenInputIsNull()
        {
            // Act
            string result = _encryptionConvertor.ConvertToProviderExpression.Compile().Invoke(null);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertFromProvider_ShouldReturnEmptyString_WhenInputIsNull()
        {
            // Act
            string result = _encryptionConvertor.ConvertFromProviderExpression.Compile().Invoke(null);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertToProvider_ShouldReturnEmptyString_WhenInputIsEmptyString()
        {
            // Act
            string result = _encryptionConvertor.ConvertToProviderExpression.Compile().Invoke(string.Empty);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertFromProvider_ShouldReturnEmptyString_WhenInputIsEmptyString()
        {
            // Act
            string result = _encryptionConvertor.ConvertFromProviderExpression.Compile().Invoke(string.Empty);

            // Assert
            result.Should().Be(string.Empty);
        }








        [Fact]
        public void ConvertFromProvider_Encrypt_ShouldReturnEmptyString_WhenInputIsEmptyString()
        {
            // Act
            string result = EncryptionConvertor.EncryptFunc(EncryptionKey, string.Empty);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertToProvider_Encrypt_ShouldReturnEmptyString_WhenInputIsNull()
        {
            // Act
            string result = EncryptionConvertor.EncryptFunc(EncryptionKey, null);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertToProvider_Encrypt_ShouldEncryptValue()
        {
            // Arrange
            string plainText = "plain text";

            // Act
            string result = EncryptionConvertor.EncryptFunc(EncryptionKey, plainText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().NotBe(plainText);
        }

        [Fact]
        public void ConvertToProvider_Decrypt_ShouldReturnEmptyString_WhenInputIsEmptyString()
        {
            // Act
            string result = EncryptionConvertor.DecryptFunc(EncryptionKey, string.Empty);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertToProvider_Decrypt_ShouldReturnEmptyString_WhenInputIsNull()
        {
            // Act
            string result = EncryptionConvertor.DecryptFunc(EncryptionKey, null);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertFromProvider_Decrypt_ShouldDecryptValue()
        {
            // Arrange
            string encryptedText = "ngcvfFH0W2iqKypGqln0HGTHMmSgqcksFLjQAVuKKEuUX3YN+/Q=";
            string plainText = "plain text";

            // Act
            string result = EncryptionConvertor.DecryptFunc(EncryptionKey, encryptedText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().Be(plainText);
        }






    }
}



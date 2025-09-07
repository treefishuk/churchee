using Churchee.Data.EntityFramework.Admin.Converters;
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
            var plainText = "plain text";

            // Act
            var result = _encryptionConvertor.ConvertToProviderExpression.Compile().Invoke(plainText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().NotBe(plainText);
        }

        [Fact]
        public void ConvertFromProvider_ShouldDecryptValue()
        {
            // Arrange
            var encryptedText = "ngcvfFH0W2iqKypGqln0HGTHMmSgqcksFLjQAVuKKEuUX3YN+/Q=";
            var plainText = "plain text";

            // Act
            var result = _encryptionConvertor.ConvertFromProviderExpression.Compile().Invoke(encryptedText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().Be(plainText);
        }

        [Fact]
        public void ConvertToProvider_ShouldReturnEmptyString_WhenInputIsNull()
        {
            // Act
            var result = _encryptionConvertor.ConvertToProviderExpression.Compile().Invoke(null);

            // Assert
            result.Should().Be(String.Empty);
        }

        [Fact]
        public void ConvertFromProvider_ShouldReturnEmptyString_WhenInputIsNull()
        {
            // Act
            var result = _encryptionConvertor.ConvertFromProviderExpression.Compile().Invoke(null);

            // Assert
            result.Should().Be(String.Empty);
        }

        [Fact]
        public void ConvertToProvider_ShouldReturnEmptyString_WhenInputIsEmptyString()
        {
            // Act
            var result = _encryptionConvertor.ConvertToProviderExpression.Compile().Invoke(string.Empty);

            // Assert
            result.Should().Be(String.Empty);
        }

        [Fact]
        public void ConvertFromProvider_ShouldReturnEmptyString_WhenInputIsEmptyString()
        {
            // Act
            var result = _encryptionConvertor.ConvertFromProviderExpression.Compile().Invoke(string.Empty);

            // Assert
            result.Should().Be(String.Empty);
        }
    }
}



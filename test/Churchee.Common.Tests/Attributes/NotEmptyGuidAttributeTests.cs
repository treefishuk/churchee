using System.ComponentModel.DataAnnotations;

namespace Churchee.Common.Tests.Attributes
{
    public class NotEmptyGuidAttributeTests
    {
        private readonly NotEmptyGuidAttribute _attribute = new NotEmptyGuidAttribute();

        [Fact]
        public void IsValid_ReturnsTrue_ForNonEmptyGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            bool result = _attribute.IsValid(guid);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_ForEmptyGuid()
        {
            // Arrange
            var guid = Guid.Empty;

            // Act
            bool result = _attribute.IsValid(guid);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_ForNonGuidType()
        {
            // Arrange
            string value = "not a guid";

            // Act
            bool result = _attribute.IsValid(value);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ReturnsFalse_ForNull()
        {
            // Act
            bool result = _attribute.IsValid(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void FormatErrorMessage_ReturnsExpectedMessage()
        {
            // Arrange
            string propertyName = "TestProperty";

            // Act
            string message = _attribute.FormatErrorMessage(propertyName);

            // Assert
            Assert.Equal("TestProperty must not be an empty GUID.", message);
        }
    }
}
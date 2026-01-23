using Churchee.Common.Attributes;
using System.Reflection;

namespace Churchee.Common.Tests.Attributes
{
    public class EncryptPropertyAttributeTests
    {

        [Fact]
        public void EncryptPropertyAttribute_HasCorrectAttributeUsage()
        {
            // Arrange
            var attributeType = typeof(EncryptPropertyAttribute);
            var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();

            // Assert
            Assert.NotNull(attributeUsage);
            Assert.Equal(AttributeTargets.Property, attributeUsage.ValidOn);
            Assert.False(attributeUsage.AllowMultiple);
            Assert.False(attributeUsage.Inherited);
        }
    }
}

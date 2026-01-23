using Churchee.Module.Tokens.Entities;
using Churchee.Test.Helpers.Validation;
using Xunit;

namespace Churchee.Module.Tokens.Tests.Entities
{
    public class TokenTests
    {
        private readonly Guid _applicationTenantId;
        private readonly string _key;
        private readonly string _value;

        public TokenTests()
        {
            _applicationTenantId = Guid.NewGuid();
            _key = "TestKey";
            _value = "TestValue";
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Act
            var token = new Token(_applicationTenantId, _key, _value);

            // Assert
            token.ApplicationTenantId.Should().Be(_applicationTenantId);
            token.Key.Should().Be(_key);
            token.Value.Should().Be(_value);
        }
    }
}


using Churchee.Module.Identity.Entities;
using FluentAssertions;

namespace Churchee.Module.Identity.Tests.Entities
{
    public class ApplicationUserTests
    {
        [Fact]
        public void ApplicationUser_Constructor_SetsProperties()
        {
            //arrange
            var applicationTenantId = Guid.NewGuid();
            string userName = "test-user";
            string email = "test@churchee.com";

            //act
            var cut = new ApplicationUser(applicationTenantId, userName, email);

            //assert
            cut.ApplicationTenantId.Should().Be(applicationTenantId);
            cut.UserName.Should().Be(userName);
            cut.Email.Should().Be(email);

        }
    }
}

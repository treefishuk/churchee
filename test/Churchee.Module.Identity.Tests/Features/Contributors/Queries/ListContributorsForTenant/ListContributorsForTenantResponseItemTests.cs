using Churchee.Module.Identity.Features.Contributors.Queries;
using FluentAssertions;

namespace Churchee.Module.Identity.Tests.Features.Contributors.Queries.ListContributorsForTenant
{
    public class ListContributorsForTenantResponseItemTests
    {
        [Fact]
        public void ListContributorsForTenantResponseItemTests_PorpertiesSet()
        {
            //arrange
            var createdDate = DateTime.Now;
            var id = Guid.NewGuid();
            bool lockedOut = true;
            string userName = "testUserName";

            //act
            var cut = new ListContributorsForTenantResponseItem()
            {
                CreatedDate = createdDate,
                Id = id,
                LockedOut = lockedOut,
                UserName = userName
            };

            //assert
            cut.CreatedDate.Should().Be(createdDate);
            cut.Id.Should().Be(id);
            cut.LockedOut.Should().Be(lockedOut);
            cut.UserName.Should().Be(userName);
        }
    }
}

using Churchee.Module.Events.Features.Queries;
using FluentAssertions;

namespace Churchee.Module.Events.Tests.Features.Queries.GetDetailById
{
    public class GetDetailByIdQueryTests
    {
        [Fact]
        public void GetDetailByIdQuery_ParametersSet()
        {
            //arrange
            var id = Guid.NewGuid();

            //act
            var cut = new GetDetailByIdQuery(id);

            //assert
            cut.Id.Should().Be(id);
        }
    }
}

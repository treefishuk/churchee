using Churchee.Module.Logging.Features.Queries;

namespace Churchee.Module.Logging.Tests.Features.Queries.GetLogDetails
{
    public class GetLogDetailsQueryTests
    {
        [Fact]
        public void GetLogDetailsQueryConstructorTests()
        {
            // Arrange
            int id = 3;

            // Act
            var query = new GetLogDetailsQuery(id);

            // Assert
            Assert.Equal(id, query.Id);
        }

    }
}

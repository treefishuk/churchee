namespace Churchee.Module.X.Tests.Features.Tweets.Commands.EnableTweetsSync
{
    using Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync;
    using Xunit;

    public class GetAccountIdApiResponseTests
    {
        [Fact]
        public void GetId_ReturnsId_WhenDataIsPresent()
        {
            // Arrange
            var response = new GetAccountIdApiResponse
            {
                Data = new Data { Id = "12345" }
            };

            // Act
            var id = response.GetId();

            // Assert
            Assert.Equal("12345", id);
        }

        [Fact]
        public void GetId_ReturnsEmptyString_WhenDataIsNotPresent()
        {
            // Arrange
            var response = new GetAccountIdApiResponse
            {
                Data = null
            };

            // Act
            var id = response.GetId();

            // Assert
            Assert.Equal(string.Empty, id);
        }

        [Fact]
        public void Data_DefaultConstructor_InitializesPropertiesToEmpty()
        {
            // Arrange
            var data = new Data();

            // Assert
            Assert.Equal(string.Empty, data.Id);
            Assert.Equal(string.Empty, data.Name);
            Assert.Equal(string.Empty, data.Username);
        }

        [Fact]
        public void GetAccountIdApiResponse_DefaultConstructor_InitializesData()
        {
            // Arrange
            var response = new GetAccountIdApiResponse();

            // Assert
            Assert.NotNull(response.Data);
            Assert.Equal(string.Empty, response.Data.Id);
        }
    }

}

namespace Churchee.Module.X.Tests.Features.Tweets.Queries.CheckXIntergrationStatus
{
    using Churchee.Module.X.Features.Tweets.Queries;
    using System;
    using Xunit;

    public class CheckXIntegrationStatusResponseTests
    {
        [Fact]
        public void Default_Properties_Are_Default()
        {
            var response = new CheckXIntegrationStatusResponse();

            Assert.False(response.Configured);
            Assert.Null(response.LastRun);
        }

        [Fact]
        public void Can_Set_And_Get_Properties()
        {
            var now = DateTime.UtcNow;
            var response = new CheckXIntegrationStatusResponse
            {
                Configured = true,
                LastRun = now
            };

            Assert.True(response.Configured);
            Assert.Equal(now, response.LastRun);
        }
    }

}

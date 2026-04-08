using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.ChurchSuite.Features.Commands.EnableChurchSuiteIntegration;
using Churchee.Test.Helpers;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Net;

namespace Churchee.Module.ChurchSuite.Tests.Features.Commands.EnableChurchSuiteIntegration
{
    public class EnableChurchSuiteIntegrationCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Enable_Integration()
        {
            // Arrange
            var command = new EnableChurchSuiteIntegrationCommand("demo.churchsuite.com");
            var settingStoreMock = new Mock<ISettingStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var mockClientFactory = new Mock<IHttpClientFactory>();
            var jobServiceMock = new Mock<IJobService>();


            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            mockClientFactory.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            var cut = new EnableChurchSuiteIntegrationCommandHandler(settingStoreMock.Object, currentUserMock.Object, mockClientFactory.Object, jobServiceMock.Object);

            // Act
            var response = await cut.Handle(command, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeTrue();
        }
    }
}

﻿using Churchee.Module.Identity.Features.HIBP.Queries;
using FluentAssertions;
using Moq;
using System.Net;

namespace Churchee.Module.Identity.Tests.Features.HIBP.Queries.CheckPasswordAgainstHIBP
{
    public class CheckPasswordAgainstHIBPCommandHandlerTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly CheckPasswordAgainstHIBPCommandHandler _handler;

        public CheckPasswordAgainstHIBPCommandHandlerTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _handler = new CheckPasswordAgainstHIBPCommandHandler(_httpClientFactoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenPasswordIsEmpty()
        {
            // Arrange
            var command = new CheckPasswordAgainstHIBPCommand(string.Empty);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Errors.Should().ContainSingle(e => e.Description == "No Password Provided" && e.Property == "Password");
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenApiCallFails()
        {
            // Arrange
            var command = new CheckPasswordAgainstHIBPCommand("password");
            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.InternalServerError));
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Errors.Should().ContainSingle(e => e.Description == "Error Calling hibp API" && e.Property == "Password");
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenPasswordIsFoundInHIBP()
        {
            // Arrange
            var command = new CheckPasswordAgainstHIBPCommand("password");
            var responseContent = "1E4C9B93F3F0682250B6CF8331B7EE68FD8:1000\r\n";
            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, responseContent));
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Errors.Should().ContainSingle(e => e.Description.Contains("Password found in leaked passwords database (HIBP)"));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPasswordIsNotFoundInHIBP()
        {
            // Arrange
            var command = new CheckPasswordAgainstHIBPCommand("password");
            var responseContent = "1F4C9B93F3F0681150B6CF8331B7EE68FD8:0\r\n";
            var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, responseContent));
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Errors.Should().BeEmpty();
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;
            private readonly string _responseContent;

            public FakeHttpMessageHandler(HttpStatusCode statusCode, string responseContent = "")
            {
                _statusCode = statusCode;
                _responseContent = responseContent;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(_responseContent)
                };
                return Task.FromResult(response);
            }
        }
    }

}

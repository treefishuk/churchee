using Churchee.Common.ResponseTypes;
using Churchee.Presentation.Admin.PipelineBehaviours;
using Churchee.Test.Helpers.Validation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Churchee.Presentation.Admin.Tests.PipelineBehaviours
{
    public class ExceptionHandlingBehaviorTests
    {
        // Make these types accessible so Moq / Castle DynamicProxy can create ILogger<T> proxies.
        public record TestRequest() : IRequest<TestResponse>;
        public class TestResponse : CommandResponse { }

        public record RequestWithNoDefaultCtor() : IRequest<NoDefaultCtorResponse>;
        public class NoDefaultCtorResponse : CommandResponse
        {
            // No parameterless ctor - Activator.CreateInstance<T>() will throw
            public NoDefaultCtorResponse(string value) { }
        }

        [Fact]
        public async Task Handle_ShouldReturnNextResponse_WhenNoException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ExceptionHandlingBehavior<TestRequest, TestResponse>>>();
            var behavior = new ExceptionHandlingBehavior<TestRequest, TestResponse>(loggerMock.Object);

            var expected = new TestResponse();

            Task<TestResponse> next(CancellationToken _ = default)
            {
                return Task.FromResult(expected);
            }

            // Act
            var actual = await behavior.Handle(new TestRequest(), next, CancellationToken.None);

            // Assert
            actual.Should().Be(expected);
            actual.IsSuccess.Should().BeTrue();

            loggerMock.Verify(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnCommandResponseWithError_WhenNextThrows()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ExceptionHandlingBehavior<TestRequest, TestResponse>>>();
            loggerMock.Setup(l => l.IsEnabled(LogLevel.Error)).Returns(true);

            var behavior = new ExceptionHandlingBehavior<TestRequest, TestResponse>(loggerMock.Object);

            var thrown = new InvalidOperationException("boom");

            Task<TestResponse> next(CancellationToken _ = default)
            {
                throw thrown;
            }

            // Act
            var result = await behavior.Handle(new TestRequest(), next, CancellationToken.None);

            // Assert - we received a TResponse created by the behavior with an error
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Count.Should().NotBe(0);
            result.Errors[0].Description.Should().Contain("Oh dear something has gone wrong");

            loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(ex => ex == thrown),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_ShouldRethrow_WhenCannotConstructTResponse()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ExceptionHandlingBehavior<RequestWithNoDefaultCtor, NoDefaultCtorResponse>>>();
            loggerMock.Setup(l => l.IsEnabled(LogLevel.Error)).Returns(true);

            var behavior = new ExceptionHandlingBehavior<RequestWithNoDefaultCtor, NoDefaultCtorResponse>(loggerMock.Object);

            var original = new InvalidOperationException("original");

            Task<NoDefaultCtorResponse> next(CancellationToken _ = default)
            {
                throw original;
            }

            // Act / Assert - since Activator.CreateInstance<TResponse>() will throw, behavior should ultimately rethrow original
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await behavior.Handle(new RequestWithNoDefaultCtor(), next, CancellationToken.None));

            // Ensure create-time failure (or original error) produced log activity
            loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
        }
    }
}
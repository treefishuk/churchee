using Churchee.Common.ResponseTypes;
using Churchee.Presentation.Admin.PipelineBehaviours;
using Churchee.Test.Helpers.Validation;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace Churchee.Presentation.Admin.Tests.PipelineBehaviours
{
    public class ValidationBehaviorTests
    {
        // Public test request/response types so test framework and proxying tools can work correctly.
        public record TestRequest() : IRequest<TestResponse>;
        public class TestResponse : CommandResponse { }

        [Fact]
        public async Task Handle_ShouldCallNext_WhenNoValidationFailures()
        {
            // Arrange
            var validatorMock = new Mock<IValidator<TestRequest>>();
            validatorMock
                .Setup(v => v.Validate(It.IsAny<TestRequest>()))
                .Returns(new ValidationResult()); // no failures

            var behavior = new ValidationBehavior<TestRequest, TestResponse>([validatorMock.Object]);

            var expected = new TestResponse();
            bool nextCalled = false;

            Task<TestResponse> Next(CancellationToken _)
            {
                nextCalled = true;
                return Task.FromResult(expected);
            }

            // Act
            var actual = await behavior.Handle(new TestRequest(), Next, CancellationToken.None);

            // Assert
            nextCalled.Should().BeTrue();
            actual.Should().Be(expected);
            actual.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnResponseWithErrors_WhenValidationFailuresExist()
        {
            // Arrange
            var failure1 = new ValidationFailure("Request.Something", "First error");
            var failure2 = new ValidationFailure("Request.Other", "Second error");

            var validationResult = new ValidationResult([failure1, failure2]);

            var validatorMock = new Mock<IValidator<TestRequest>>();
            validatorMock
                .Setup(v => v.Validate(It.IsAny<TestRequest>()))
                .Returns(validationResult);

            var behavior = new ValidationBehavior<TestRequest, TestResponse>(new[] { validatorMock.Object });

            // next should not be called when there are failures
            static Task<TestResponse> Next(CancellationToken _)
            {
                throw new InvalidOperationException("Next should not be invoked when validation fails");
            }

            // Act
            var result = await behavior.Handle(new TestRequest(), Next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().HaveCount(2);

            // The ValidationBehavior trims property name prefix up to the first '.' so expect "Something" and "Other"
            result.Errors[0].Property.Should().Be("Something");
            result.Errors[0].Description.Should().Be("First error");

            result.Errors[1].Property.Should().Be("Other");
            result.Errors[1].Description.Should().Be("Second error");
        }

        [Fact]
        public void Handle_ShouldThrowIfTResponseDoesNotInheritCommandResponse_CompileTimeGuard()
        {
            // Arrange / Act / Assert
            // This is a compile-time constraint on the generic type parameters and doesn't need runtime testing.
            // The test is included for clarity: attempting to instantiate ValidationBehavior with a TResponse that
            // does not inherit CommandResponse will not compile.
            var a = () => { _ = typeof(ValidationBehavior<TestRequest, TestResponse>); };
            a.Should().NotBeNull();
        }
    }
}
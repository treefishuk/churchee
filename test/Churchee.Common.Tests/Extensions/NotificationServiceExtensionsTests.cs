using Churchee.Common.ResponseTypes;
using Churchee.Test.Helpers.Validation;
using Radzen;

namespace Churchee.Common.Tests.Extensions
{
    public class NotificationServiceExtensionsTests
    {
        [Fact]
        public void NotificationServiceExtensionsTests_NotifyWithSuccessMessage_ShouldReturnASuccessMessage()
        {
            // Arrange
            var service = new NotificationService();
            var commandResponse = new CommandResponse();
            var successMessage = "Whoop Whoop";

            // Act
            service.Notify(commandResponse, successMessage);

            // Assert
            service.Messages.First().Severity.Should().Be(NotificationSeverity.Success);
            service.Messages.First().Summary.Should().Be("Whoop Whoop");
        }

        [Fact]
        public void NotificationServiceExtensionsTests_NotifyWithSuccessMessage_ShouldReturnErrorMessagesWhenFails()
        {
            // Arrange
            var service = new NotificationService();
            var commandResponse = new CommandResponse();
            commandResponse.AddError("Failure", "Here");
            var successMessage = "Whoop Whoop";

            // Act
            service.Notify(commandResponse, successMessage);

            // Assert
            service.Messages.First().Severity.Should().Be(NotificationSeverity.Error);
            service.Messages.First().Summary.Should().Be("Failure");
        }

        [Fact]
        public void NotificationServiceExtensionsTests_Notify_ShouldReturnASuccessMessage()
        {
            // Arrange
            var service = new NotificationService();
            var commandResponse = new CommandResponse();

            // Act
            service.Notify(commandResponse);

            // Assert
            service.Messages.First().Severity.Should().Be(NotificationSeverity.Success);
            service.Messages.First().Summary.Should().Be("Saved Successfully");

        }

        [Fact]
        public void NotificationServiceExtensionsTests_Notify_ShouldReturnFailureMessage()
        {
            // Arrange
            var service = new NotificationService();
            var commandResponse = new CommandResponse();
            commandResponse.AddError("Failure", "Here");

            // Act
            service.Notify(commandResponse);

            // Assert
            service.Messages.First().Severity.Should().Be(NotificationSeverity.Error);
            service.Messages.First().Summary.Should().Be("Failure");
        }

    }
}

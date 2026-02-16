using Bunit;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Radzen;

namespace Churchee.Module.UI.Tests
{
    public class ModalFormTests : BasePageTests
    {
        public ModalFormTests()
        {
            // Provide Radzen NotificationService (simple concrete instance)
            Services.AddSingleton<NotificationService>(new NotificationService());

            // Provide a minimal IServiceProvider for ValidationContext (can be the test services provider)
            Services.AddSingleton<IServiceProvider>(sp => Services);

            // If ModalForm or child components require other services, register them here as needed.
        }

        [Fact]
        public void CancelButton_Invokes_OnCancelForm()
        {
            // Arrange
            var model = new TestModel { Name = "abc" };
            bool cancelled = false;
            var onCancel = EventCallback.Factory.Create(this, () => cancelled = true);

            // Act
            var cut = Render<ModalForm>(parameters => parameters
                .Add(p => p.InputModel, model)
                .Add(p => p.OnCancelForm, onCancel)
            );

            // Find and click the Cancel button (RadzenButton renders a button element)
            var cancelButton = cut.FindAll("button").First(b => b.TextContent?.Contains("Cancel") == true);
            cancelButton.Click();

            // Assert
            cancelled.Should().BeTrue();
        }

        [Fact]
        public void Submit_ValidModel_Invokes_OnSave()
        {
            // Arrange
            var model = new TestModel { Name = "abc" };
            bool saved = false;
            var onSave = EventCallback.Factory.Create<object>(this, _ => saved = true);

            var cut = Render<ModalForm>(parameters => parameters
                .Add(p => p.InputModel, model)
                .Add(p => p.OnSave, onSave)
            );

            // Act - click the Submit button
            var submitButton = cut.FindAll("button").First(b => b.TextContent?.Contains("Submit") == true);
            submitButton.Click();

            // Assert
            saved.Should().BeTrue();
        }

        [Fact]
        public async Task OnSaveException_IsCaught_And_LoggedAsError()
        {
            // Arrange
            var model = new TestModel { Name = "abc" };

            // Mock ILogger<Form> to verify an Error-level log occurs when OnSave throws
            var mockLogger = new Mock<ILogger<Form>>();
            Services.AddSingleton(mockLogger.Object);

            // OnSave that throws
            var onSave = EventCallback.Factory.Create<object>(this, async _ =>
            {
                await Task.Yield();
                throw new InvalidOperationException("boom");
            });

            var cut = Render<ModalForm>(parameters => parameters
                .Add(p => p.InputModel, model)
                .Add(p => p.OnSave, onSave)
            );

            // Act - click Submit
            var submitButton = cut.FindAll("button").First(b => b.TextContent?.Contains("Submit") == true);
            submitButton.Click();

            // Give the async flow a moment to complete
            await Task.Delay(50);

            // Assert - verify logger was used to log an Error (calls to ILogger are done via extension methods;
            // verify the underlying Log invocation with LogLevel.Error occurred)
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("Error Submitting form")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        private class TestModel
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
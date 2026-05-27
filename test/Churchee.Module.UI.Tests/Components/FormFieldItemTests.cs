using Bunit;
using Churchee.Common.ValueTypes;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Radzen;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.UI.Tests.Components
{
    public class FormFieldItemTests : BasePageTests
    {
        private readonly Mock<ILogger> _mockLogger;

        public FormFieldItemTests()
        {
            _mockLogger = new Mock<ILogger>();
            Services.AddSingleton(_mockLogger.Object);
        }

        private class TestModel
        {
            // Ensure the component picks up the ImageUpload path:
            // Use the string-based DataType attribute to set the custom data type to DataTypes.ImageUpload.
            [DataType(DataTypes.ImageUpload)]
            public Upload Photo { get; set; } = new Upload();
        }

        [Fact]
        public void ImageUpload_Renders_InputFile()
        {
            // Arrange
            var model = new TestModel();
            var editContext = new EditContext(model);
            var prop = typeof(TestModel).GetProperty(nameof(TestModel.Photo))!;

            // Act
            var cut = Render<FormFieldItem>(parameters => parameters
                .Add(p => p.Prop, prop)
                .Add(p => p.InputModel, model)
                .Add(p => p.EditContext, editContext)
                .Add(p => p.OnValueChanged, EventCallback.Factory.Create<object>(this, (_) => { }))
            );

            // Assert: RadzenFileInput should render an <input type="file"> element
            var fileInput = cut.FindAll("input[type='file']");
            Assert.NotEmpty(fileInput);
        }

        [Fact]
        public void OnFileError_Notifies_When_FileTooLarge()
        {
            // Arrange
            var model = new TestModel();
            var editContext = new EditContext(model);
            var prop = typeof(TestModel).GetProperty(nameof(TestModel.Photo))!;

            var cut = Render<FormFieldItem>(parameters => parameters
                .Add(p => p.Prop, prop)
                .Add(p => p.InputModel, model)
                .Add(p => p.EditContext, editContext)
                .Add(p => p.OnValueChanged, EventCallback.Factory.Create<object>(this, (_) => { }))
            );

            // Use reflection to invoke the private OnFileError method
            var instance = cut.Instance;

            // Create args that contain the "File too large" message to trigger the specific branch
            var uploadErrorType = typeof(UploadErrorEventArgs);
            var args = Activator.CreateInstance(uploadErrorType) as UploadErrorEventArgs;
            args!.Message = "File too large";

            // Act
            cut.Instance.OnFileError(args, "FileInput");

            // Assert
            NotificationService.Notifications.Should().ContainSingle(n => n.Severity == NotificationSeverity.Error && n.Summary.StartsWith("File size to large"));


        }
    }
}
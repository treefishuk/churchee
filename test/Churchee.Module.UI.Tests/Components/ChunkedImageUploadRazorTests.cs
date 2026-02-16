using Bunit;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ValueTypes;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;
using Radzen.Blazor;

namespace Churchee.Module.UI.Tests.Components
{
    public class ChunkedImageUploadRazorTests : BasePageTests
    {


        public ChunkedImageUploadRazorTests()
        {
            var mockImageProcessor = new Mock<IImageProcessor>();
            mockImageProcessor.Setup(x => x.ResizeImageAsync(It.IsAny<Stream>(), 300, 0, ".webp", It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new MemoryStream([1, 2, 3]));

            Services.AddSingleton(mockImageProcessor.Object);

            var mockAiToolUtilities = new Mock<IAiToolUtilities>();
            mockAiToolUtilities.Setup(x => x.GenerateAltTextAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync("generated alt");
            Services.AddSingleton(mockAiToolUtilities.Object);
        }

        [Fact]
        public async Task Upload_sets_thumbnail_and_description()
        {
            // Arrange
            var model = new ChunkedImageUploadType();

            var editContext = new EditContext(model);

            // Render the component with a cascading EditContext
            var wrap = Render<CascadingValue<EditContext>>(parameters => parameters
                .Add(p => p.Value, editContext)
                .AddChildContent<ChunkedImageUpload>(parameters => parameters.Add(p => p.Model, model))
            );

            var cut = wrap.FindComponent<ChunkedImageUpload>();

            byte[] fileBytes = [0x1, 0x2];

            // Act
            var input = cut.FindComponent<InputFile>();

            input.UploadFiles(InputFileContent.CreateFromBinary(fileBytes, "photo.jpg", null, "image/jpeg"));

            // Assert
            model.ThumbnailUrl.Should().NotBeNull();
            model.ThumbnailUrl.Should().NotBeEmpty();
            model.Description.Should().Be("generated alt");

            // Cleanup temp file
            if (!string.IsNullOrEmpty(model.TempFilePath) && File.Exists(model.TempFilePath))
            {
                File.Delete(model.TempFilePath);
            }
        }


        [Fact]
        public async Task Upload_progress_initially_0()
        {
            // Arrange
            var model = new ChunkedImageUploadType();

            var editContext = new EditContext(model);

            // Render the component with a cascading EditContext
            var wrap = Render<CascadingValue<EditContext>>(parameters => parameters
                .Add(p => p.Value, editContext)
                .AddChildContent<ChunkedImageUpload>(parameters => parameters.Add(p => p.Model, model))
            );

            var cut = wrap.FindComponent<ChunkedImageUpload>();

            // Act
            var progressBar = cut.FindComponent<RadzenProgressBar>();

            // Assert
            progressBar.Instance.Value.Should().Be(0);
            progressBar.Instance.ProgressBarStyle.Should().Be(ProgressBarStyle.Primary);
        }

        [Fact]
        public async Task Upload_sets_progress()
        {
            // Arrange
            var model = new ChunkedImageUploadType();

            var editContext = new EditContext(model);

            // Render the component with a cascading EditContext
            var wrap = Render<CascadingValue<EditContext>>(parameters => parameters
                .Add(p => p.Value, editContext)
                .AddChildContent<ChunkedImageUpload>(parameters => parameters.Add(p => p.Model, model))
            );

            var cut = wrap.FindComponent<ChunkedImageUpload>();

            byte[] fileBytes = [0x1, 0x2];

            // Act
            var input = cut.FindComponent<InputFile>();

            input.UploadFiles(InputFileContent.CreateFromBinary(fileBytes, "photo.jpg", null, "image/jpeg"));

            // Assert

            var progressBar = cut.FindComponent<RadzenProgressBar>();

            // Assert
            progressBar.Instance.Value.Should().Be(100);
            progressBar.Instance.ProgressBarStyle.Should().Be(ProgressBarStyle.Success);

            // Cleanup temp file
            if (!string.IsNullOrEmpty(model.TempFilePath) && File.Exists(model.TempFilePath))
            {
                File.Delete(model.TempFilePath);
            }
        }


    }
}

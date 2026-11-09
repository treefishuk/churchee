using System.Reflection;
using Bunit;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;

namespace Churchee.Module.UI.Tests.Components
{
    public class ImageUploadTests : BasePageTests
    {
        private static Mock<IBrowserFile> CreateBrowserFile(string name, string contentType, byte[] bytes)
        {
            var file = new Mock<IBrowserFile>();
            file.SetupGet(x => x.Name).Returns(name);
            file.SetupGet(x => x.Size).Returns(bytes.LongLength);
            file.SetupGet(x => x.ContentType).Returns(contentType);
            file.Setup(x => x.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(() => new MemoryStream(bytes));
            return file;
        }

        [Fact]
        public async Task OnImageUploadInputChangeAsync_UsesTenantResolver_StoresUploadData_AndInvokesModelChanged()
        {
            var tenantId = Guid.NewGuid();
            var tenantResolver = new Mock<ITenantResolver>();
            tenantResolver.Setup(x => x.GetTenantId()).Returns(tenantId);

            var imageProcessor = new Mock<IImageProcessor>();
            imageProcessor.Setup(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Stream _, CancellationToken _) => new MemoryStream([1, 2, 3, 4]));
            imageProcessor.Setup(x => x.ResizeImageAsync(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Stream _, int _, int _, string _, CancellationToken _) => new MemoryStream([9, 9, 9]));

            var blobStore = new Mock<IBlobStore>();
            blobStore.Setup(x => x.SaveAsync(tenantId, It.IsAny<string>(), It.IsAny<Stream>(), false, It.IsAny<CancellationToken>()))
                .ReturnsAsync("images/generated/test.webp");

            var jobService = new Mock<IJobService>();

            Services.AddSingleton(tenantResolver.Object);
            Services.AddSingleton(imageProcessor.Object);
            Services.AddSingleton(blobStore.Object);
            Services.AddSingleton(jobService.Object);

            var model = new ImageUploadType { Path = "/img/test", Description = "Example", ShowDescription = false };
            ImageUploadType? callbackModel = null;

            var cut = Render<ImageUpload>(parameters => parameters
                .Add(p => p.Model, model)
                .Add(p => p.ModelChanged, EventCallback.Factory.Create<ImageUploadType>(this, value => callbackModel = value)));

            var file = CreateBrowserFile("example.png", "image/png", [10, 20, 30]);
            var args = new UploadChangeEventArgs { Files = [new Radzen.FileInfo(file.Object)] };

            var method = typeof(ImageUpload).GetMethod("OnImageUploadInputChangeAsync", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);

            await cut.InvokeAsync(async () => await ((Task)method.Invoke(cut.Instance, [model, args])!));

            Assert.Equal(tenantId, model.ApplicationTenantId);
            Assert.Equal("images/generated/test.webp", model.FileName);
            Assert.Equal("image/png", model.ContentType);
            Assert.NotNull(model.File);
            Assert.Equal("example.png", model.File.Name);
            Assert.Contains("data:image/webp;base64,", model.ThumbnailUrl);
            Assert.Same(model, callbackModel);
        }

        [Fact]
        public async Task OnDescriptionChange_InvokesModelChanged()
        {
            var model = new ImageUploadType { Description = "hello", ShowDescription = false };
            ImageUploadType? callbackModel = null;

            var cut = Render<ImageUpload>(parameters => parameters
                .Add(p => p.Model, model)
                .Add(p => p.ModelChanged, EventCallback.Factory.Create<ImageUploadType>(this, value => callbackModel = value)));

            var method = typeof(ImageUpload).GetMethod("OnDescriptionChange", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);

            await cut.InvokeAsync(async () => await ((Task)method.Invoke(cut.Instance, null!)!));

            Assert.Same(model, callbackModel);
        }

        [Fact]
        public async Task ToBase64ImageAsync_EncodesStreamAsDataUri()
        {
            await using var stream = new MemoryStream([0x00, 0x01, 0x02, 0x03]);

            var result = await ImageUpload.ToBase64ImageAsync(stream, "image/png");

            Assert.Equal("data:image/png;base64,AAECAw==", result);
        }

        [Fact]
        public async Task GenerateThumbnailPreviewAsync_WhenStreamIsEmpty_ReturnsEmptyString()
        {
            var cut = Render<ImageUpload>(parameters => parameters.Add(p => p.Model, new ImageUploadType { ShowDescription = false }));

            var method = typeof(ImageUpload).GetMethod("GenerateThumbnailPreviewAsync", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);

            var result = await (Task<string>)method.Invoke(cut.Instance, [new MemoryStream(), 180, CancellationToken.None])!;

            Assert.Equal(string.Empty, result);
        }
    }
}

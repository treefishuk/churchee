using Churchee.Module.Site.Features.Favicons.Commands;
using FluentValidation.TestHelper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Churchee.Module.Site.Tests.Features.Favicons
{
    public class GenerateFaviconCommandValidatorTests
    {
        private static string CreateBase64Png(int width, int height)
        {
            using var image = new Image<Rgba32>(width, height);
            using var ms = new MemoryStream();
            image.Save(ms, new PngEncoder());
            string base64 = Convert.ToBase64String(ms.ToArray());
            return $"data:image/png;base64,{base64}";
        }

        private static string CreateBase64Jpeg(int width, int height)
        {
            using var image = new Image<Rgba32>(width, height);
            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);
            string base64 = Convert.ToBase64String(ms.ToArray());
            return $"data:image/jpeg;base64,{base64}";
        }

        [Fact]
        public void ValidPngSquareAndLargeEnough_ShouldBeValid()
        {
            string base64 = CreateBase64Png(600, 600);
            var command = new GenerateFaviconCommand(base64, Guid.NewGuid());
            var validator = new GenerateFaviconCommandValidator();

            var result = validator.TestValidate(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void NotAnImage_ShouldBeInvalid()
        {
            string base64 = "data:image/png;base64,notarealbase64string";
            var command = new GenerateFaviconCommand(base64, Guid.NewGuid());
            var validator = new GenerateFaviconCommandValidator();

            var result = validator.TestValidate(command);

            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("doesn't appear to be an image"));
        }

        [Fact]
        public void NotPng_ShouldBeInvalid()
        {
            string base64 = CreateBase64Jpeg(600, 600);
            var command = new GenerateFaviconCommand(base64, Guid.NewGuid());
            var validator = new GenerateFaviconCommandValidator();

            var result = validator.TestValidate(command);

            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Must be a png"));
        }

        [Fact]
        public void NotSquare_ShouldBeInvalid()
        {
            string base64 = CreateBase64Png(600, 400);
            var command = new GenerateFaviconCommand(base64, Guid.NewGuid());
            var validator = new GenerateFaviconCommandValidator();

            var result = validator.TestValidate(command);

            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("square"));
        }

        [Fact]
        public void TooSmall_ShouldBeInvalid()
        {
            string base64 = CreateBase64Png(400, 400);
            var command = new GenerateFaviconCommand(base64, Guid.NewGuid());
            var validator = new GenerateFaviconCommandValidator();

            var result = validator.TestValidate(command);

            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("more then 575px"));
        }
    }
}
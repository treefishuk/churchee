using Churchee.ImageProcessing.Validation;
using FluentValidation;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Churchee.Module.Site.Features.Favicons.Commands
{

    public class GenerateFaviconCommandValidator : AbstractValidator<GenerateFaviconCommand>
    {
        public GenerateFaviconCommandValidator()
        {
            RuleFor(x => x.Base64Content)
                .Must(ImageValidation.BeValidImage)
                .WithMessage("Uploaded file doesn't appear to be an image.");

            RuleFor(x => x.Base64Content)
                .Must((image, base64Image) => ImageValidation.BeExpectedFormat(base64Image, ".png"))
                .WithMessage("Must be a png");

            RuleFor(x => x.Base64Content)
                .Must(BeSquare)
                .WithMessage("Image must be square");

            RuleFor(x => x.Base64Content)
                .Must(BeMoreThen575)
                .WithMessage("Image must be more then 575px");
        }

        private bool BeSquare(string base64Image)
        {
            try
            {
                string base64 = base64Image.Contains(",") ? base64Image.Split(',')[1] : base64Image;
                byte[] imageBytes = Convert.FromBase64String(base64);
                using var ms = new System.IO.MemoryStream(imageBytes);
                using var image = Image.Load<Rgba32>(ms);
                return image.Width == image.Height;
            }
            catch
            {
                return false;
            }
        }

        private bool BeMoreThen575(string base64Image)
        {
            try
            {
                string base64 = base64Image.Contains(",") ? base64Image.Split(',')[1] : base64Image;
                byte[] imageBytes = Convert.FromBase64String(base64);
                using var ms = new System.IO.MemoryStream(imageBytes);
                using var image = Image.Load<Rgba32>(ms);
                return image.Width > 575;
            }
            catch
            {
                return false;
            }
        }

    }
}

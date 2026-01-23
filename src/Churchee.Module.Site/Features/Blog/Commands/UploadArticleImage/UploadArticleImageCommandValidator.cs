using Churchee.Common.Validation;
using Churchee.ImageProcessing.Validation;
using FluentValidation;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class UploadArticleImageCommandValidator : AbstractValidator<UploadArticleImageCommand>
    {
        public UploadArticleImageCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.FileName)
                .Matches(@"^[a-zA-Z0-9_\-]+$")
                .WithMessage("File name contains invalid characters. Only letters, numbers, underscores, hyphens, and a single dot are allowed.");

            RuleFor(x => x.Description).NotEmpty();

            RuleFor(x => x.FileExtension)
                .Must(m => FileValidation.ImageFormats.Contains(m))
                .WithMessage("File format is not supported.");

            RuleFor(x => x.Base64Content)
                .Must(ImageValidation.BeValidImage)
                .WithMessage("Uploaded file doesn't appear to be an image.");

            RuleFor(x => x.Base64Content)
                .Must((image, base64Image) => ImageValidation.BeExpectedFormat(base64Image, image.FileExtension))
                .WithMessage("File extension does not match image format");


        }
    }
}

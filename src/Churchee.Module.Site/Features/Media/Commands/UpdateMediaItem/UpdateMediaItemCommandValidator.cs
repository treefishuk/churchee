using Churchee.Common.Validation;
using Churchee.ImageProcessing.Validation;
using FluentValidation;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class UpdateMediaItemCommandValidator : AbstractValidator<UpdateMediaItemCommand>
    {
        public UpdateMediaItemCommandValidator()
        {
            string[] imageFormats = { ".jpg", ".jpeg", ".png", ".webp" };
            string[] allowedFormats = { ".jpg", ".jpeg", ".png", ".webp", ".mp3", ".pdf", ".mp4" };

            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.Description).NotEmpty();

            RuleFor(x => x.FileExtension)
                .Must(m => allowedFormats.Contains(m))
                .WithMessage("File format is not supported.");

            RuleFor(x => x.Base64Content)
                .Must(ImageValidation.BeValidImage)
                .When(w => imageFormats.Contains(w.FileExtension))
                .WithMessage("Uploaded file doesn't appear to be an image.");

            RuleFor(x => x.Base64Content)
                .Must((image, base64Image) => ImageValidation.BeExpectedFormat(base64Image, image.FileExtension))
                .When(w => imageFormats.Contains(w.FileExtension))
                .WithMessage("File extension does not match image format");

            RuleFor(x => x.Base64Content)
                .Must(FileValidation.BeValidPdf).When(w => w.FileExtension == ".pdf")
                .WithMessage("File does not match PDF format");

            RuleFor(x => x.Base64Content)
                .Must(FileValidation.BeValidMp3).When(w => w.FileExtension == ".mp3")
                .WithMessage("File does not match MP3 format");

            RuleFor(x => x.Base64Content)
                .Must(FileValidation.BeValidMp4).When(w => w.FileExtension == ".mp4")
                .WithMessage("File does not match MP3 format");
        }

    }
}

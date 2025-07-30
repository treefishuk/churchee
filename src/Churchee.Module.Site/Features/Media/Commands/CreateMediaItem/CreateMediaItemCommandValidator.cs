using Churchee.ImageProcessing.Validation;
using FluentValidation;

namespace Churchee.Module.Site.Features.Media.Commands
{

    public class CreateMediaItemCommandValidator : AbstractValidator<CreateMediaItemCommand>
    {
        public CreateMediaItemCommandValidator()
        {

            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.Description).NotEmpty();

            RuleFor(x => x.FileExtension).Must(ImageValidation.BeAValidImageExtension)
                .When(w => w.SupportedFileTypes.Contains(".png") || w.SupportedFileTypes.Contains(".jpg"))
                .WithMessage("only Jpeg and PNG images are supported");

            RuleFor(x => x.Base64Image)
                .Must(ImageValidation.BeValidImage)
                .When(w => w.SupportedFileTypes.Contains(".png") || w.SupportedFileTypes.Contains(".jpg"))
                .WithMessage("Uploaded file doesn't appear to be an image.");

            RuleFor(x => x.Base64Image)
                .Must((image, base64Image) => ImageValidation.BeExpectedFormat(base64Image, image.FileExtension))
                .When(w => w.SupportedFileTypes.Contains(".png") || w.SupportedFileTypes.Contains(".jpg"))
                .WithMessage("File extension does not match image format");
        }

    }
}

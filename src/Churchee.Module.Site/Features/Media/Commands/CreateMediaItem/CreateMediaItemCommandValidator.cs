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

            RuleFor(x => x.FileExtension).Must(ImageValidation.BeAValidImageExtension).WithMessage("only Jpeg and PNG images are supported");

            RuleFor(x => x.Base64Image)
                .Must(ImageValidation.BeValidImage)
                .WithMessage("Uploaded file doesn't appear to be an image.");

            RuleFor(x => x.Base64Image)
                //.NotEmpty().WithMessage("Base64Image cannot be empty")
                //.When(x => string.IsNullOrEmpty(x.Base64Image))
                .Must((image, base64Image) => ImageValidation.BeExpectedFormat(base64Image, image.FileExtension))
                .WithMessage("File extention does not match image format");
        }

    }
}

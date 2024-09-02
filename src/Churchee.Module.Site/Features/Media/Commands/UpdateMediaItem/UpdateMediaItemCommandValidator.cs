using Churchee.ImageProcessing.Validation;
using FluentValidation;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class UpdateMediaItemCommandValidator : AbstractValidator<UpdateMediaItemCommand>
    {
        public UpdateMediaItemCommandValidator()
        {
            RuleFor(x => x.Extention).Must(ImageValidation.BeAValidImageExtension).WithMessage("only Jpeg and PNG images are supported");

            RuleFor(x => x.Base64Image)
                .Must(ImageValidation.BeValidImage)
                .WithMessage("Uploaded file doesn't appear to be an image.");

            RuleFor(x => x.Base64Image)
                .Must((image, base64Image) => ImageValidation.BeExpectedFormat(base64Image, image.Extention))
                .WithMessage("File extention does not match image format");
        }

    }
}

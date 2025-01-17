using Churchee.ImageProcessing.Validation;
using FluentValidation;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class UpdateMediaItemCommandValidator : AbstractValidator<UpdateMediaItemCommand>
    {
        public UpdateMediaItemCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.Description).NotEmpty();

            RuleFor(x => x.Extension)
                .Must((command, extension) => string.IsNullOrEmpty(extension) || ImageValidation.BeAValidImageExtension(extension)).WithMessage("only Jpeg and PNG images are supported");

            RuleFor(x => x.Base64Image)
                .Must((command, base64Image) => string.IsNullOrEmpty(base64Image) || ImageValidation.BeValidImage(base64Image))
                .WithMessage("Uploaded file doesn't appear to be an image.");

            RuleFor(x => x.Base64Image)
                .Must((command, base64Image) => string.IsNullOrEmpty(base64Image) || ImageValidation.BeExpectedFormat(base64Image, command.Extension))
                .WithMessage("File extention does not match image format");
        }

    }
}

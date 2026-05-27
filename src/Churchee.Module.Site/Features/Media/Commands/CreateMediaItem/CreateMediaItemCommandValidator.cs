using Churchee.Common.Storage;
using Churchee.Common.Validation;
using Churchee.ImageProcessing.Validation;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using FluentValidation;

namespace Churchee.Module.Site.Features.Media.Commands
{

    public class CreateMediaItemCommandValidator : AbstractValidator<CreateMediaItemCommand>
    {

        private readonly IDataStore _dataStore;

        public CreateMediaItemCommandValidator(IDataStore dataStore)
        {
            _dataStore = dataStore;

            RuleFor(x => x)
                .NotEmpty()
                .MustAsync(AllowedFileType)
                .WithMessage("File format is not supported");

            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.FileName)
                .Matches(@"^[a-zA-Z0-9_\-]+$")
                .WithMessage("File name contains invalid characters. Only letters, numbers, underscores, hyphens, and a single dot are allowed.");

            RuleFor(x => x.Description).NotEmpty();

            RuleFor(x => x.Base64Content)
                .Must(ImageValidation.BeValidImage)
                .When(w => FileValidation.ImageFormats.Contains(w.FileExtension))
                .WithMessage("Uploaded file doesn't appear to be an image.");

            RuleFor(x => x.Base64Content)
                .Must((image, base64Image) => ImageValidation.BeExpectedFormat(base64Image, image.FileExtension))
                .When(w => FileValidation.ImageFormats.Contains(w.FileExtension))
                .WithMessage("File extension does not match image format");

            RuleFor(x => x.Base64Content)
                .Must(FileValidation.BeValidPdf).When(w => w.FileExtension == ".pdf")
                .WithMessage("File does not match PDF format");

            RuleFor(x => x.Base64Content)
                .Must(FileValidation.BeValidMp3).When(w => w.FileExtension == ".mp3")
                .WithMessage("File does not match MP3 format");

            RuleFor(x => x.Base64Content)
                .Must(FileValidation.BeValidMp4).When(w => w.FileExtension == ".mp4")
                .WithMessage("File does not match MP4 format");
        }


        private async Task<bool> AllowedFileType(CreateMediaItemCommand command, CancellationToken ct)
        {
            string allowedFileExtensions = await _dataStore.GetRepository<MediaFolder>().FirstOrDefaultAsync(new MediaFolderByIdSpecification(command.FolderId.Value), w => w.SupportedFileTypes, ct);

            if (string.IsNullOrEmpty(allowedFileExtensions))
            {
                return FileValidation.AllowedFormats.Contains(command.FileExtension);
            }

            // Split the allowed file extensions by commas
            string[] extensions = allowedFileExtensions.Split(',');

            return extensions
                .Any(ext => command.FileExtension
                    .ToLower()
                    .EndsWith(ext.Trim().ToLower()));
        }
    }
}

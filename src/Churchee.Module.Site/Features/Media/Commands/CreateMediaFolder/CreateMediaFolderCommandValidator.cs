using FluentValidation;

namespace Churchee.Module.Site.Features.Media.Commands
{

    public class CreateMediaFolderCommandValidator : AbstractValidator<CreateMediaFolderCommand>
    {
        public CreateMediaFolderCommandValidator()
        {
            RuleFor(x => x.ParentId).NotNull();
        }

    }
}

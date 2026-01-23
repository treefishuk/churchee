using Churchee.Module.Site.Features.Pages.Validation;
using FluentValidation;

namespace Churchee.Module.Site.Features.Pages.Commands.UpdatePage
{

    public class UpdatePageCommandHandlerValidator : AbstractValidator<UpdatePageCommand>
    {
        public UpdatePageCommandHandlerValidator()
        {
            RuleFor(x => x.Content).Must(PageValidation.DoesNotContainScriptTags).WithMessage("Script Tags are not allowed");
            RuleFor(x => x.Content).Must(PageValidation.DoesNotContainEmbedTags).WithMessage("Embeds/iframes are only supported for youtube videos with privacy-enhanced mode enabled");
        }

    }
}

using Churchee.Module.Site.Features.Templates.Validation;
using FluentValidation;

namespace Churchee.Module.Site.Features.Templates.Commands.UpdateTemplateContent
{

    public class UpdateTemplateContentComandValidator : AbstractValidator<UpdateTemplateContentComand>
    {
        public UpdateTemplateContentComandValidator()
        {
            RuleFor(x => x.Content).Must(TemplateValidation.NotContainsDisallowedContent).WithMessage("Template contains dissallowed content");
            RuleFor(x => x.Content).Must(TemplateValidation.NotContainStyleTags).WithMessage("Please use the Styles section for css");
            RuleFor(x => x.Content).Must(TemplateValidation.NotContainsDisallowedInjects).WithMessage("Template contains an inject call that is not supported");
            RuleFor(x => x.Content).Must(TemplateValidation.NotContainsDisallowedEmbeds).WithMessage("Embeds/iframes are only supported for youtube");
        }

    }
}

using Churchee.Common.Storage;
using FluentValidation;

namespace Churchee.Module.Site.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommandValidator : AbstractValidator<CreatePageCommand>
    {
        public CreatePageCommandValidator(IDataStore storage)
        {
            RuleFor(m => m.Title).NotEmpty();
        }
    }
}

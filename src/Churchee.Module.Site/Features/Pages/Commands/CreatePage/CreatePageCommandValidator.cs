using Churchee.Common.Storage;
using FluentValidation;

namespace Churchee.Module.Site.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommandValidator : AbstractValidator<CreatePageCommand>
    {
        private readonly IDataStore _storage;

        public CreatePageCommandValidator(IDataStore storage)
        {
            _storage = storage;

            RuleFor(m => m.Title).NotEmpty();
        }
    }
}

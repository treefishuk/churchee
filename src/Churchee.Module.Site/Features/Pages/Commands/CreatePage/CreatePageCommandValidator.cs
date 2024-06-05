using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private bool BeValidUrlAsync(string url)
        {
            if (url == null) return false;

            return url.StartsWith("/");
        }

    }
}

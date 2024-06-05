using Churchee.Common.Abstractions.Auth;
using FluentValidation;

namespace Churchee.Module.Tenancy.Features.Churches.Commands.AddChurch
{
    public class AddChurchCommandValidator : AbstractValidator<AddChurchCommand>
    {

        private readonly ICurrentUser _currentUser;

        public AddChurchCommandValidator(ICurrentUser currentUser)
        {
            _currentUser = currentUser;

            RuleFor(x => x.Name).Custom((command, context) =>
            {

                if (!currentUser.HasRole("SysAdmin"))
                {
                    context.AddFailure("Sorry, you do not have the permission required for that action");
                }
            });
        }

    }
}

using Churchee.Common.Abstractions.Auth;

namespace Churchee.Module.Tenancy.Features.Churches.Commands.AddChurch
{
    public class AddChurchCommandValidator : AbstractValidator<AddChurchCommand>
    {
        public AddChurchCommandValidator(ICurrentUser currentUser)
        {
            this.RuleFor(x => x.Name).Custom((command, context) =>
            {

                if (!currentUser.HasRole("SysAdmin"))
                {
                    context.AddFailure("Sorry, you do not have the permission required for that action");
                }
            });
        }

    }
}

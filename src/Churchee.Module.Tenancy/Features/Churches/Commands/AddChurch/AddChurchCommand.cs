using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Tenancy.Features.Churches.Commands
{
    public class AddChurchCommand : IRequest<CommandResponse>
    {
        public AddChurchCommand(string name, int charityNumber)
        {
            Name = name;
            CharityNumber = charityNumber;
        }

        public string Name { get; private set; }

        public int CharityNumber { get; private set; }

    }
}

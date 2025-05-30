using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Tenancy.Features.Churches.Commands
{
    public class UpdateChurchCommand : IRequest<CommandResponse>
    {
        public UpdateChurchCommand(Guid id, int? charityNumber, List<string> domains)
        {
            Id = id;
            CharityNumber = charityNumber;
            Domains = domains ?? new List<string>();
        }

        public Guid Id { get; set; }

        public int? CharityNumber { get; set; }

        public List<string> Domains { get; set; }
    }
}

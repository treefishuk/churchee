using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Commands.CreatePageTypeContent
{
    public class CreatePageTypeContentCommand : IRequest<CommandResponse>
    {
        public CreatePageTypeContentCommand(Guid pageTypeId, string name, string type, bool required, int order)
        {
            PageTypeId = pageTypeId;
            Name = name;
            Type = type;
            Required = required;
            Order = order;
        }

        public Guid PageTypeId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool Required { get; set; }

        public int Order { get; set; }

    }
}

using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Commands.CreatePageTypeContent
{
    public class CreatePageTypeContentComand : IRequest<CommandResponse>
    {
        public CreatePageTypeContentComand(Guid pageTypeId, string name, string type, bool required)
        {
            PageTypeId = pageTypeId;
            Name = name;
            Type = type;
            Required = required;
        }

        public Guid PageTypeId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool Required { get; set; }

    }
}

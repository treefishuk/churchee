using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class UpdateMediaItemCommand : IRequest<CommandResponse>
    {
        public UpdateMediaItemCommand(Guid id, string name, string fileName, string extention, string linkUrl, string description, string additionalContent, string base64Image)
        {
            Id = id;
            Name = name;
            FileName = fileName;
            Extention = extention;
            LinkUrl = linkUrl;
            Description = description;
            AdditionalContent = additionalContent;
            Base64Image = base64Image;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string Extention { get; set; }

        public string LinkUrl { get; set; }

        public string Description { get; set; }

        public string AdditionalContent { get; set; }

        public string Base64Image { get; set; }
    }
}

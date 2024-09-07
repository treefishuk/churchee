using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class CreateMediaItemCommand : IRequest<CommandResponse>
    {
        public CreateMediaItemCommand(string name, string fileName, string extention, string description, string additionalContent, Guid? folderId, string base64Image, string linkUrl, string cssClass)
        {
            Name = name;
            FileName = fileName;
            Extention = extention;
            Description = description;
            AdditionalContent = additionalContent;
            FolderId = folderId;
            Base64Image = base64Image;
            LinkUrl = linkUrl;
            CssClass = cssClass;
        }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string Extention { get; set; }

        public string LinkUrl { get; set; }

        public string Description { get; set; }

        public string AdditionalContent { get; set; }

        public string Base64Image { get; set; }

        public Guid? FolderId { get; set; }

        public string CssClass { get; set; }


    }
}

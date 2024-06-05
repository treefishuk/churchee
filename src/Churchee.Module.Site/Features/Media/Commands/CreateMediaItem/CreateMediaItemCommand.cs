using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.ResponseTypes;
using Churchee.Common.ValueTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class CreateMediaItemCommand : IRequest<CommandResponse>
    {
        public CreateMediaItemCommand(string name, string extention, string description, string additionalContent, Guid? folderId, string base64Image, string linkUrl)
        {
            Name = name;
            Extention = extention;
            Description = description;
            AdditionalContent = additionalContent;
            FolderId = folderId;
            Base64Image = base64Image;
            LinkUrl = linkUrl;
        }

        public string Name { get; set; }

        public string Extention { get; set; }

        public string LinkUrl { get; set; }

        public string Description { get; set; }

        public string AdditionalContent { get; set; }

        public string Base64Image { get; set; }

        public Guid? FolderId { get; set; }

    }
}

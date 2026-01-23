using Churchee.Common.ResponseTypes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Site.Features.Templates.Commands.UpdateTemplateContent
{
    public class UpdateTemplateContentComand : IRequest<CommandResponse>
    {
        public UpdateTemplateContentComand(Guid templateId, string content)
        {
            TemplateId = templateId;
            Content = content;
        }

        public Guid TemplateId { get; set; }

        public string Content { get; set; }

    }
}

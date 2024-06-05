using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Areas.Site.Models;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Commands.UpdatePageTypeContent
{
    public class UpdatePageTypeContentCommand : IRequest<CommandResponse>
    {
        public UpdatePageTypeContentCommand(Guid pageId, List<PageTypeContentItemModel> content)
        {
            PageTypeId = pageId;
            Content = content;
        }

        public Guid PageTypeId { get; set; }

        public List<PageTypeContentItemModel> Content { get; set; }

    }
}

using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class CreateArticleCommand : IRequest<CommandResponse>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string TempImagePath { get; set; }

        public string ImagePath { get; set; }

        public string ImageFileName { get; set; }

        public string ImageAltTag { get; set; }

        public DateTime? PublishOnDate { get; set; }

        public Guid ParentPageId { get; set; }
    }
}

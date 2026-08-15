using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Features.Blog.Responses
{
    public class GetListBlogItemsResponseItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public bool Published { get; set; }

        [DataType(DataTypes.Date)]
        public DateTime? PublishDate { get; set; }

        [DataType(DataTypes.Date)]
        public DateTime? Modified { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Podcasts.Features.Queries
{
    public class GetListingQueryResponseItem
    {
        public GetListingQueryResponseItem()
        {
            ImageUri = string.Empty;
            Title = string.Empty;
            Source = string.Empty;
        }

        [DataType(DataTypes.Hidden)]
        public Guid Id { get; set; }

        [DataType(DataTypes.ImageUrl)]
        public string ImageUri { get; set; }

        public string Title { get; set; }

        public string Source { get; set; }

        public bool Active { get; set; }

        [DataType(DataTypes.Date)]
        public DateTime PublishedDate { get; set; }

    }
}

using Churchee.Common.ValueTypes;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class PageEditModel
    {
        public PageEditModel()
        {
            Title = string.Empty;
            Description = string.Empty;
            Parent = new DropdownInput();
            Order = 10;
            ContentItems = new List<GetPageDetailsResponseContentItem>();
            Image = new ChunkedImageUploadType() { SupportedFileTypes = ".jpg,.jpeg,.png,.gif", TempFilePath = string.Empty, Path = "pages/", ShowDescription = false, ThumbnailUrl = "/_content/Churchee.Module.UI/img/opengraph-placeholder.png" };
        }

        [MaxLength(100)]
        [DataType(DataTypes.TextWithSlug)]
        [Required]
        public string Title { get; set; }

        [MaxLength(200)]
        [DataType(DataTypes.MultilineText)]
        public string Description { get; set; }

        public DropdownInput Parent { get; set; }

        [Required]
        public int Order { get; set; }

        public List<GetPageDetailsResponseContentItem> ContentItems { get; set; }

        [DataType(DataTypes.ChunkedImageUpload)]
        public ChunkedImageUploadType Image { get; set; }

    }
}

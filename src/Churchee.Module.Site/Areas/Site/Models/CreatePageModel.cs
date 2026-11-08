using Churchee.Common.ValueTypes;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{

    public class CreatePageModel
    {
        public CreatePageModel()
        {
            Title = string.Empty;
            Description = string.Empty;
            PageType = new DropdownInput();
            Parent = new DropdownInput();
            Order = 10;
            Image = new ImageUploadType()
            {
                SupportedFileTypes = ".jpg,.jpeg,.png,.gif",
                Path = "/img/pages",
                ThumbnailUrl = "/_content/Churchee.Module.UI/img/opengraph-placeholder.png"
            };
        }

        [Required]
        [DataType(DataTypes.TextWithSlug)]
        public string Title { get; set; }

        [Required]
        [MaxLength(200)]
        [DataType(DataTypes.MultilineText)]
        public string Description { get; set; }

        [Required]
        public DropdownInput PageType { get; set; }

        [Required]
        public DropdownInput Parent { get; set; }

        [Required]
        public int Order { get; set; }

        [DataType(DataTypes.ImageUpload)]
        public ImageUploadType Image { get; set; }

    }
}

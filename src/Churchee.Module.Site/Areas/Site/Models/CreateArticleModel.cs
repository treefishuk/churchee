using Churchee.Common.ValueTypes;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{

    public class CreateArticleModel
    {
        public CreateArticleModel()
        {
            Title = string.Empty;
            Description = string.Empty;
            Content = "<p></p>";
            Image = new ChunkedUploadType() { SupportedFileTypes = ".jpg,.jpeg,.png,.gif", FileName = string.Empty, Path = "articles/" };
        }

        [Required]
        [DataType(DataTypes.TextWithSlug)]
        public string Title { get; set; }

        [Required]
        [MaxLength(200)]
        [DataType(DataTypes.MultilineText)]
        public string Description { get; set; }

        [Required]
        public DropdownInput Parent { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PublishOnDate { get; set; }

        [DataType(DataTypes.ChunkedImageUpload)]
        public ChunkedUploadType Image { get; set; }

        [Required]
        [DataType(DataTypes.HtmlTall)]
        public string Content { get; set; }


    }
}

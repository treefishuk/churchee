using Churchee.Common.ValueTypes;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{

    public class EditArticleModel
    {
        public EditArticleModel()
        {
            Title = string.Empty;
            Description = string.Empty;
            Content = "<p></p>";
            Image = new ChunkedImageUploadType() { SupportedFileTypes = ".jpg,.jpeg,.png,.gif", TempFilePath = string.Empty, Path = "articles/" };
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
        public ChunkedImageUploadType Image { get; set; }

        [Required]
        [DataType(DataTypes.HtmlTall)]
        public string Content { get; set; }


    }
}

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


        [Required]
        [DataType(DataTypes.Html)]
        public string Content { get; set; }


    }
}

using System.ComponentModel.DataAnnotations;
using Churchee.Module.UI.Models;

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

    }
}

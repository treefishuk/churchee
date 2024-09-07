using Churchee.Common.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class CreateMediaItemModel
    {
        public CreateMediaItemModel()
        {
            Name = string.Empty;
            Description = string.Empty;
            AdditionalContent = string.Empty;
            LinkUrl = string.Empty;
            CssClass = string.Empty;
            File = new Upload();
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [ValidateComplexType]
        [DataType(DataTypes.ImageUpload)]
        public Upload File { get; set; }

        [DataType(DataTypes.Url)]
        public string LinkUrl { get; set; }

        [Display(Name = "Class")]
        [RegularExpression(pattern: RegexPattern.SingleLowercaseWord, ErrorMessage = "Must be a single camelcase string")]
        public string CssClass { get; set; }

        [DataType(DataTypes.Html)]
        public string AdditionalContent { get; set; }

    }
}

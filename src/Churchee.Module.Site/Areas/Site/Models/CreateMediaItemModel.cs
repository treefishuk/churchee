using System.ComponentModel.DataAnnotations;
using Churchee.Common.ValueTypes;

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
            File = new Upload();
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [ValidateComplexType]
        [DataType(DataTypes.Upload)]
        public Upload File { get; set; }

        [DataType(DataTypes.Url)]
        public string LinkUrl { get; set; }

        [DataType(DataTypes.Html)]
        public string AdditionalContent { get; set; }

    }
}

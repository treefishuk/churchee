using Churchee.Common.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class CreateFaviconModel
    {
        public CreateFaviconModel()
        {
            File = new Upload();
        }

        [Required]
        [ValidateComplexType]
        [DataType(DataTypes.MediaUpload)]
        public Upload File { get; set; }
    }
}

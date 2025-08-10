using Churchee.Common.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.UI.Models
{
    public class ImageUploadModel
    {

        public ImageUploadModel()
        {
            File = new Upload();
            Name = string.Empty;
            Description = string.Empty;
        }

        [Required]
        [ValidateComplexType]
        [DataType(DataTypes.MediaUpload)]
        public Upload File { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

    }
}

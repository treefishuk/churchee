using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.YouTube.Areas.Integrations.Models
{
    public class InputModel
    {
        public InputModel()
        {
            Handle = string.Empty;
            ApiKey = string.Empty;
            NameForContent = string.Empty;
        }

        [Required]
        [DataType(DataType.Text)]
        public string Handle { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string NameForContent { get; set; }

        [Required]
        [DataType(DataTypes.Password)]
        public string ApiKey { get; set; }
    }
}

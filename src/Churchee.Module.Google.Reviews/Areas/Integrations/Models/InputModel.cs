using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Google.Reviews.Areas.Integrations.Models
{
    public class InputModel
    {
        public InputModel()
        {
            ClientId = string.Empty;
            ClientSecret = string.Empty;
            BusinessProfileId = string.Empty;
        }

        [Required]
        public string ClientId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ClientSecret { get; set; }

        [Required]
        public string BusinessProfileId { get; set; }

    }
}

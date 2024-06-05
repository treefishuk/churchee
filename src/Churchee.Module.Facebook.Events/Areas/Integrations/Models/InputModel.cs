using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Facebook.Events.Areas.Integrations.Models
{
    public class InputModel
    {
        public InputModel()
        {
            FacebookPageId = string.Empty;
        }

        [Required]
        public string FacebookPageId { get; set; }

    }
}

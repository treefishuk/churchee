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
        [RegularExpression(@"^(?:\d{5,20})$", ErrorMessage = "Enter a valid Facebook numeric page ID")]
        [Display(Name = "Facebook Page ID", GroupName = "Configuration", Description = "Find in: Page Name → About → More info")]
        public string FacebookPageId { get; set; }

    }
}

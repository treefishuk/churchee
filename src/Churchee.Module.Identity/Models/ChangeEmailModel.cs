using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Identity.Models
{
    public class ChangeEmailModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }
}

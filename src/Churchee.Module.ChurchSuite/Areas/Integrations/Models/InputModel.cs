using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.ChurchSuite.Events.Areas.Integrations.Models
{
    public class InputModel
    {
        [Required]
        [RegularExpression(@"^[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.churchsuite\.com", ErrorMessage = "Enter a valid churchsuite sub domain is required")]

        [Display(Name = "Sub Domain", GroupName = "Configuration", Description = "Ex. demo.churchsuite.com")]
        public string SubDomain { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Identity.Models
{
    public class EnableMultiFactorModel
    {
        [Required]
        public string Code { get; set; }
    }
}

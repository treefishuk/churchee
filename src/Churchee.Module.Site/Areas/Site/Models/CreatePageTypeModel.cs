using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class CreatePageTypeModel
    {
        public CreatePageTypeModel()
        {
            Name = string.Empty;
        }

        [Required]
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Tenancy.Areas.Configuration.Churches.Create
{
    public class CreateChurchInputModel
    {

        public CreateChurchInputModel()
        {
            Name = string.Empty;
        }

        [Required]
        [RegularExpression(pattern: RegexPattern.BasicText, ErrorMessage = "only spaces, apostrophe, uppercase and lowercase letters supported")]
        public string Name { get; set; }

        public int? CharityNumber { get; set; }

    }
}

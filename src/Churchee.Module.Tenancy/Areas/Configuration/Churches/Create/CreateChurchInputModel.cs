using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Tenancy.Areas.Configuration.Churches.Create
{
    public class CreateChurchInputModel
    {

        public CreateChurchInputModel()
        {
            Name = string.Empty;
            CharityNumber = string.Empty;
        }

        [Required]
        public string Name { get; set; }

        public string CharityNumber { get; set; }

    }
}

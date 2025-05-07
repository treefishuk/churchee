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
        public string Name { get; set; }

        public int? CharityNumber { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.X.Areas.Integrations.Models
{
    public class InputModel
    {
        public InputModel()
        {
            BearerToken = string.Empty;
            AccountName = string.Empty;
        }

        [Required]
        [DataType(DataType.Password)]
        public string BearerToken { get; set; }

        [Required]
        [DataType(DataTypes.Text)]
        public string AccountName { get; set; }
    }
}

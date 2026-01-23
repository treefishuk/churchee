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
        [Display(GroupName = "Configuration", Description = "Find in: App → Settings → Keys and Tokens")]
        [DataType(DataType.Password)]
        public string BearerToken { get; set; }

        [Required]
        [DataType(DataTypes.Text)]
        [Display(GroupName = "Configuration", Description = "ex. @myawesomechurch")]
        public string AccountName { get; set; }
    }
}

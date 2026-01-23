using Churchee.Common.ValueTypes;
using Churchee.Module.Identity.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Identity.Models
{
    public class CreateContributorModel
    {

        public CreateContributorModel(MultiSelect roles)
        {
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            Roles = roles;
        }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataTypes.Password)]
        [Display(Name = "Password")]
        [PasswordRequirements]
        public string Password { get; set; }

        [DataType(DataTypes.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataTypes.CheckboxList)]
        public MultiSelect Roles { get; set; }

    }
}

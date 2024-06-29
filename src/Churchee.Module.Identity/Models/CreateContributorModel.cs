using Churchee.Common.ValueTypes;
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
        [StringLength(maximumLength: 100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        [DataType(DataTypes.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataTypes.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataTypes.CheckboxList)]
        public MultiSelect Roles { get; set; }

    }
}

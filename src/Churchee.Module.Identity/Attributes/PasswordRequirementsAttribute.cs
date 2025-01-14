using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Churchee.Module.Identity.Attributes
{
    public partial class PasswordRequirementsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required.");
            }

            var identityOptions = validationContext.GetService<IOptions<IdentityOptions>>()?.Value.Password;

            if (identityOptions == null)
            {
                return new ValidationResult("Password validation options are not configured.");
            }

            if (identityOptions.RequireUppercase && !UppercaseRegex().IsMatch(password))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (identityOptions.RequireLowercase && !LowercaseRegex().IsMatch(password))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            if (identityOptions.RequireDigit && !DigitRegex().IsMatch(password))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }

            if (identityOptions.RequireNonAlphanumeric && !SpecialCharRegex().IsMatch(password))
            {
                return new ValidationResult("Password must contain at least one special character.");
            }

            if (password.Length < identityOptions.RequiredLength)
            {
                return new ValidationResult($"Password must be at least {identityOptions.RequiredLength} characters long.");
            }

            return ValidationResult.Success;
        }

        [GeneratedRegex(@"[A-Z]")]
        private static partial Regex UppercaseRegex();

        [GeneratedRegex(@"[a-z]")]
        private static partial Regex LowercaseRegex();

        [GeneratedRegex(@"[0-9]")]
        private static partial Regex DigitRegex();

        [GeneratedRegex(@"[\W_]")]
        private static partial Regex SpecialCharRegex();
    }
}

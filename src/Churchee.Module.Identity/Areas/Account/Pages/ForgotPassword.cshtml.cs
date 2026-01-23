using Churchee.Common.Abstractions.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Areas.Account.Pages
{
    [AllowAnonymous]
    public sealed class ForgotPasswordModel : PageModel
    {
        private readonly ChurcheeUserManager _userManager;
        private readonly IEmailService _emailSender;
        private readonly ILogger _logger;

        public ForgotPasswordModel(ChurcheeUserManager userManager, IEmailService emailSender, ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = default!;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                    "ResetPassword",
                    pageHandler: null,
                    values: new { area = "Account", code },
                    protocol: Request.Scheme)!;

                if (callbackUrl == null)
                {
                    _logger.LogError("Callback URL is null. Check the Url.Page method parameters and ensure the page exists.");
                    return Page();
                }

                var message = "Reset Password Link: " + callbackUrl;

                await _emailSender.SendEmailAsync(user.Email, user.UserName, "Reset your password", message, message);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}

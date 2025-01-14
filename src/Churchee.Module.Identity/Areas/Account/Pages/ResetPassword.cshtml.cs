// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Churchee.Module.Identity.Attributes;
using Churchee.Module.Identity.Features.HIBP.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Areas.Account.Pages
{

    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly ChurcheeUserManager _userManager;
        private readonly IMediator _mediator;

        public ResetPasswordModel(ChurcheeUserManager userManager, IMediator mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = default!;

            [Required]
            [DataType(DataType.Password)]
            [PasswordRequirements]
            public string Password { get; set; } = default!;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Code { get; set; } = default!;
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var hibpResult = await _mediator.Send(new CheckPasswordAgainstHIBPCommand(Input.Password));

            if (!hibpResult.IsSuccess)
            {
                ModelState.AddModelError("Input.Password", "Password found in leaked passwords database. Please use something else");

                return Page();
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);

            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}

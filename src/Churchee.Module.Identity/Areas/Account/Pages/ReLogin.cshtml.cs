using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Areas.Account.Pages
{
    public class ReLoginModel : PageModel
    {


        private readonly ChurcheeSignInManager _signInManager;
        private readonly ChurcheeUserManager _userManager;

        public ReLoginModel(ChurcheeSignInManager signInManager, ChurcheeUserManager userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {

            string id = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null)
            {
                return new NotFoundResult();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new NotFoundResult();
            }

            await _signInManager.RefreshSignInAsync(user);

            returnUrl ??= Url.Content("~/");

            return Redirect(returnUrl);
        }
    }
}

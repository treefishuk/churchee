using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Churchee.Module.Identity.Areas.Account.Pages
{
    public class LogOutModel : PageModel
    {
        private readonly ChurcheeSignInManager _signInManager;
        private readonly ChurcheeUserManager _userManager;

        public LogOutModel(ChurcheeSignInManager signInManager, ChurcheeUserManager userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {

            string id = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null)
            {
                return Redirect("/");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Redirect("/");
            }

            await _signInManager.SignOutAsync();

            return Redirect("/");
        }
    }
}

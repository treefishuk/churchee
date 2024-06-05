using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Abstractions
{
    public interface ISignInManager
    {
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);

        Task SignOutAsync();

        bool IsSignedIn(ClaimsPrincipal principal);
    }
}

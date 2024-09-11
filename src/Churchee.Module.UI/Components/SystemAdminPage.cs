using Microsoft.AspNetCore.Authorization;

namespace Churchee.Module.UI.Components
{
    [Authorize(Roles = "SysAdmin")]
    public class SystemAdminPage : BasePage
    {
    }
}

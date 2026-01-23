using Hangfire.Dashboard;

namespace Churchee.Presentation.Admin.Filters
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}

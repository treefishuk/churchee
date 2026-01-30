using Churchee.Common.Abstractions.Extensibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Data.EntityFramework.Site.Registrations
{
    public class ServiceRegistrations : IConfigureSiteServicesAction
    {
        public int Priority => 5000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            string connectionString = configuration.GetConnectionString("Default");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string 'Default' is not configured.");
            }

            serviceCollection.AddDbContext<SiteDbContext>(options =>
                options.UseSqlServer(connectionString));

            serviceCollection.AddScoped<DbContext, SiteDbContext>();
        }
    }
}

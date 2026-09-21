using Churchee.Common.Abstractions.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Infrastructure.DatabaseCaching.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 1000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Default");
                options.SchemaName = "dbo";
                options.TableName = "Caching";
            });
        }
    }
}
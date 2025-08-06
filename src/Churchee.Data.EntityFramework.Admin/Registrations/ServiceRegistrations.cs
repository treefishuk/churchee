using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Churchee.Data.EntityFramework.Admin.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 5000;
        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {

            string connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");

            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            serviceCollection.AddTransient<DbContext>(c => c.GetService<ApplicationDbContext>());

            serviceCollection.AddTransient<IDataStore, EFStorage>();

            serviceCollection.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>()
                .ProtectKeysWithCertificate(
                    new X509Certificate2("dp.pfx", serviceProvider.GetService<IConfiguration>()["Security:DPK"]));
        }
    }
}

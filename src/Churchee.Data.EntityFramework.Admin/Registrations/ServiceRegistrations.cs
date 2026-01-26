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
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            string dpKey = config["Security:DPK"];

            var cert = X509CertificateLoader.LoadPkcs12FromFile("dp.pfx", dpKey);

            string connectionString = config.GetConnectionString("DefaultConnection");

            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            serviceCollection.AddTransient<DbContext>(c => c.GetService<ApplicationDbContext>());

            serviceCollection.AddTransient<IDataStore, EFStorage>();

            serviceCollection.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>()
                .ProtectKeysWithCertificate(cert);
        }
    }
}

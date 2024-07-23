using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Churchee.Data.EntityFramework.Registrations
{
    public class ServiceRegistrations : IConfigureServicesAction
    {
        public int Priority => 5000;
        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddTransient<DbContext, ApplicationDbContext>();

            serviceCollection.AddTransient<IDataStore, EFStorage>();

            serviceCollection.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>()
                .ProtectKeysWithCertificate(
                    new X509Certificate2("dp.pfx", serviceProvider.GetService<IConfiguration>()["Security:DPK"]));
        }
    }
}

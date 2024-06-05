using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Churchee.Data.EntityFramework
{
    public class DbContextRegistration : IConfigureServicesAction
    {
        public int Priority => 5000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddTransient<DbContext, ApplicationDbContext>();

            serviceCollection.AddTransient<IDataStore, EFStorage>();
        }
    }
}

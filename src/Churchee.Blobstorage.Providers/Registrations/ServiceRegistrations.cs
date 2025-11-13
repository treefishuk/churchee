using Churchee.Blobstorage.Providers.Azure;
using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Churchee.Blobstorage.Providers.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 10;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<IBlobStore, AzureBlobStore>();
        }
    }
}

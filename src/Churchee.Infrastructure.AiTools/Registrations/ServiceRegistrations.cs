using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Infrastructure.AiTools.Settings;
using Churchee.Infrastructure.AiTools.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Infrastructure.AiTools.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 1000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.Configure<AzureVisionSettings>(serviceProvider.GetRequiredService<IConfiguration>().GetSection("AzureVision"));

            if (serviceProvider.GetRequiredService<IConfiguration>().GetValue<bool>("UseTestAiToolUtilities"))
            {
                serviceCollection.AddScoped<IAiToolUtilities, TestAiToolUtilities>();
                return;
            }

            serviceCollection.AddScoped<IAiToolUtilities, AiToolUtilities>();
        }
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Common.Abstractions.Extensibility
{
    public interface IConfigureSiteServicesAction
    {
        int Priority { get; }

        void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider);
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Common.Abstractions.Extensibility
{
    public interface IConfigureAdminServicesAction
    {
        int Priority { get; }

        void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider);
    }
}

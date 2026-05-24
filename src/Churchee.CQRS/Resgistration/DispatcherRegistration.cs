using Churchee.CQRS.Abstractions;
using Churchee.CQRS.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Frozen;
using System.Reflection;

namespace Churchee.CQRS.Resgistration
{
    public static class Registrations
    {
        public static IServiceCollection AddDispatcher(this IServiceCollection services, Assembly[] assemblies)
        {
            var requestWrappers = new Dictionary<Type, RequestHandlerBase>();
            var notificationWrappers = new Dictionary<Type, NotificationHandlerBase>();

            foreach (var assembly in assemblies)
            {
                AddTypesFromAssembly(services, assembly, requestWrappers, notificationWrappers);
            }

            var registry = new DispatcherRegistry(
                requestWrappers.ToFrozenDictionary(),
                notificationWrappers.ToFrozenDictionary());

            services.AddSingleton(registry);
            services.AddTransient<Dispatcher>();
            services.AddTransient<ISender>(sp => sp.GetRequiredService<Dispatcher>());
            services.AddTransient<IPublisher>(sp => sp.GetRequiredService<Dispatcher>());

            return services;
        }

        private static void AddTypesFromAssembly(IServiceCollection services, Assembly assembly, Dictionary<Type, RequestHandlerBase> requestWrappers, Dictionary<Type, NotificationHandlerBase> notificationWrappers)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType)
                    {
                        continue;
                    }

                    var def = iface.GetGenericTypeDefinition();

                    if (def == typeof(IRequestHandler<,>))
                    {
                        services.AddTransient(iface, type);

                        var args = iface.GetGenericArguments();
                        var requestType = args[0];
                        var responseType = args[1];

                        if (!requestWrappers.ContainsKey(requestType))
                        {
                            var wrapperType = typeof(RequestHandlerWrapper<,>)
                                .MakeGenericType(requestType, responseType);
                            requestWrappers[requestType] =
                                (RequestHandlerBase)Activator.CreateInstance(wrapperType)!;
                        }
                    }
                    else if (def == typeof(INotificationHandler<>))
                    {
                        services.AddTransient(iface, type);

                        var notificationType = iface.GetGenericArguments()[0];
                        if (!notificationWrappers.ContainsKey(notificationType))
                        {
                            var wrapperType = typeof(NotificationHandlerWrapper<>)
                                .MakeGenericType(notificationType);
                            notificationWrappers[notificationType] =
                                (NotificationHandlerBase)Activator.CreateInstance(wrapperType)!;
                        }
                    }
                }
            }
        }
    }

}
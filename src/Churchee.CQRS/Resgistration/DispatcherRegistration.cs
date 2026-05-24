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
            var requestWrappers = new Dictionary<Type, IRequestHandlerBase>();
            var notificationWrappers = new Dictionary<Type, INotificationHandlerBase>();

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

        private static void AddTypesFromAssembly(IServiceCollection services, Assembly assembly, Dictionary<Type, IRequestHandlerBase> requestWrappers, Dictionary<Type, INotificationHandlerBase> notificationWrappers)
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
                        AddRequestHandler(services, requestWrappers, type, iface);
                    }
                    else if (def == typeof(INotificationHandler<>))
                    {
                        AddNotificationHandler(services, notificationWrappers, type, iface);
                    }
                }
            }
        }

        private static void AddNotificationHandler(IServiceCollection services, Dictionary<Type, INotificationHandlerBase> notificationWrappers, Type type, Type iface)
        {
            services.AddTransient(iface, type);

            var notificationType = iface.GetGenericArguments()[0];

            if (!notificationWrappers.ContainsKey(notificationType))
            {
                var wrapperType = typeof(NotificationHandlerWrapper<>)
                    .MakeGenericType(notificationType);
                notificationWrappers[notificationType] =
                    (INotificationHandlerBase)Activator.CreateInstance(wrapperType)!;
            }
        }

        private static void AddRequestHandler(IServiceCollection services, Dictionary<Type, IRequestHandlerBase> requestWrappers, Type type, Type iface)
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
                    (IRequestHandlerBase)Activator.CreateInstance(wrapperType)!;
            }
        }
    }

}
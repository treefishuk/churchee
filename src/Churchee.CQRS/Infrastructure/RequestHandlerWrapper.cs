using Churchee.CQRS.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.CQRS.Infrastructure
{

    /// <summary>
    /// One concrete wrapper per (TRequest, TResponse) pair. Created once at registration via
    /// Activator.CreateInstance and stored in the FrozenDictionary. The Handle method is a
    /// strongly-typed call site - no reflection, no MakeGenericType.
    /// </summary>
    internal sealed class RequestHandlerWrapper<TRequest, TResponse> : IRequestHandlerBase<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public Task<TResponse> Handle(
            IRequest<TResponse> request,
            IServiceProvider provider,
            CancellationToken cancellationToken)
        {
            var typed = (TRequest)request;
            var handler = provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            var behaviors = provider.GetServices<IPipelineBehavior<TRequest, TResponse>>();

            // Build the pipeline: handler at the core, behaviors wrapped outside in registration order.
            // Iterating in reverse means the first registered behavior runs outermost.
            RequestHandlerDelegate<TResponse> pipeline = () => handler.Handle(typed, cancellationToken);

            foreach (var behavior in behaviors.Reverse())
            {
                var next = pipeline;
                var current = behavior;
                pipeline = () => current.Handle(typed, next, cancellationToken);
            }

            return pipeline();
        }
    }
}

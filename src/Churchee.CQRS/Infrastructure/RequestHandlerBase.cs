using Churchee.CQRS.Abstractions;

namespace Churchee.CQRS.Infrastructure
{
    /// <summary>
    /// Non-generic base so it can live in a single FrozenDictionary value type.
    /// </summary>
    internal abstract class RequestHandlerBase;

    /// <summary>
    /// Generic over the response so the dispatcher can downcast and call without boxing.
    /// </summary>
    internal abstract class RequestHandlerBase<TResponse> : RequestHandlerBase
    {
        public abstract Task<TResponse> Handle(
            IRequest<TResponse> request,
            IServiceProvider provider,
            CancellationToken cancellationToken);
    }
}

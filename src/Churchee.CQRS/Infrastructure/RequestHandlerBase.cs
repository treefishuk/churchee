using Churchee.CQRS.Abstractions;

namespace Churchee.CQRS.Infrastructure
{

    /// <summary>
    /// Generic over the response so the dispatcher can downcast and call without boxing.
    /// </summary>
    internal abstract class RequestHandlerBase<TResponse> : IRequestHandlerBase
    {
        public abstract Task<TResponse> Handle(
            IRequest<TResponse> request,
            IServiceProvider provider,
            CancellationToken cancellationToken);
    }
}

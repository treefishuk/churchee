namespace Churchee.CQRS.Abstractions
{
    /// <summary>
    /// Non-generic base so it can live in a single FrozenDictionary value type.
    /// </summary>
    internal interface IRequestHandlerBase;

    /// <summary>
    /// Generic over the response so the dispatcher can downcast and call without boxing.
    /// </summary>
    internal interface IRequestHandlerBase<TResponse> : IRequestHandlerBase
    {
        public abstract Task<TResponse> Handle(
            IRequest<TResponse> request,
            IServiceProvider provider,
            CancellationToken cancellationToken);
    }
}

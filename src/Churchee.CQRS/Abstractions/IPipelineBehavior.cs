namespace Churchee.CQRS.Abstractions
{


    /// <summary>
    /// A cross-cutting behavior that wraps the handler. Behaviors are composed in
    /// registration order: the first registered runs outermost.
    /// </summary>
    public interface IPipelineBehavior<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            RequestHandler<TResponse> next,
            CancellationToken cancellationToken);
    }
}

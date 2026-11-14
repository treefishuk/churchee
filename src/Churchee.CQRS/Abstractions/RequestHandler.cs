namespace Churchee.CQRS.Abstractions
{
    /// <summary>
    /// Delegate that represents the next step in a pipeline. Calling it invokes the next
    /// behavior or, at the end of the chain, the handler itself.
    /// </summary>
    public delegate Task<TResponse> RequestHandler<TResponse>();
}

namespace Churchee.CQRS.Abstractions
{
    /// <summary>
    /// Handles a request and returns a response
    /// </summary>
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}

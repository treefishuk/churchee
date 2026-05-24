namespace Churchee.CQRS.Abstractions
{
    /// <summary>
    /// Marker for a request that returns a response. MediatR-12 compatible shape so existing
    /// handler code can move with a single using-statement swap.
    /// </summary>
    public interface IRequest<out TResponse>;
}

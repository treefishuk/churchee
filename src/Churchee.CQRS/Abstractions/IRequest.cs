namespace Churchee.CQRS.Abstractions
{
    /// <summary>
    /// Marker for a request that returns a response. MediatR-12 compatible shape so existing
    /// handler code can move with a single using-statement swap.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Required for IRequest pattern")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Required for IRequest pattern")]
    public interface IRequest<out TResponse>;
}

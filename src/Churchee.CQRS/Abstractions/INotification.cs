namespace Churchee.CQRS.Abstractions
{
    /// <summary>
    /// Marker for an in-process notification. Multiple handlers per notification are allowed.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Required for notification pattern")]
    public interface INotification;
}

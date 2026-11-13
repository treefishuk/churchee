namespace Churchee.Common.Abstractions.Queue
{
    public interface IJob
    {
        Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken);
    }
}

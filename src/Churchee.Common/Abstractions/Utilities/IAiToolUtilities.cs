namespace Churchee.Common.Abstractions.Utilities
{
    public interface IAiToolUtilities
    {
        Task<string> GenerateAltTextAsync(Stream imageStream, CancellationToken cancellationToken);
    }
}

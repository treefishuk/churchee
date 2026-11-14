namespace Churchee.Common.Abstractions.Utilities
{
    public interface ISassComplier
    {
        Task<string> CompileStringAsync(string scss, bool compressed, CancellationToken cancellationToken);
    }
}

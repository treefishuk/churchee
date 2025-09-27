using Churchee.Common.Abstractions.Utilities;

namespace Churchee.Infrastructure.AiTools.Utilities
{
    public class TestAiToolUtilities : IAiToolUtilities
    {
        public async Task<string> GenerateAltTextAsync(Stream imageStream, CancellationToken cancellationToken)
        {
            await Task.Delay(2000, cancellationToken); // Simulate some async work

            return await Task.FromResult("Generated Text");
        }
    }
}

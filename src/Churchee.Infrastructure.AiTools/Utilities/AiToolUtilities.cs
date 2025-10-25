using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Infrastructure.AiTools.Settings;
using Microsoft.Extensions.Options;

namespace Churchee.Infrastructure.AiTools.Utilities
{
    public class AiToolUtilities : IAiToolUtilities
    {

        private readonly AzureVisionSettings _settings;

        public AiToolUtilities(IOptions<AzureVisionSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<string> GenerateAltTextAsync(Stream imageStream, CancellationToken cancellationToken)
        {
            //Create Client
            var client = new ImageAnalysisClient(new Uri(_settings.Endpoint), new AzureKeyCredential(_settings.ApiKey));

            // Get a caption for the image.
            ImageAnalysisResult result = await client.AnalyzeAsync(BinaryData.FromStream(imageStream), VisualFeatures.Caption, new ImageAnalysisOptions { GenderNeutralCaption = false }, cancellationToken);

            // Return the generated caption if available
            return result.Caption.Text ?? "No caption generated";
        }
    }
}

using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Hangfire;

namespace Churchee.Module.Site.Jobs
{
    public class ImageProcessingJobBase
    {
        private readonly IDataStore _dataStore;
        private readonly IBlobStore _blobStore;
        private readonly IImageProcessor _imageProcessor;
        private readonly string _area;
        private readonly ImageCropsGenerator _imageCropsGenerator;

        public ImageProcessingJobBase(IDataStore dataStore, IBlobStore blobStore, IImageProcessor imageProcessor, IBackgroundJobClient backgroundJobClient, string area)
        {
            _dataStore = dataStore;
            _blobStore = blobStore;
            _imageProcessor = imageProcessor;
            _area = area;
            _imageCropsGenerator = new ImageCropsGenerator(blobStore, imageProcessor);
        }

        public async Task ProcessAsync(Guid applicationTenantId, Guid eventId, string imageFileName, byte[] imageData, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(imageFileName) || imageData == null || imageData.Length == 0)
            {
                return;
            }

            string fileName = Path.GetFileNameWithoutExtension(imageFileName);

            await using var ms = new MemoryStream(imageData);

            await using var webpStream = await _imageProcessor.ConvertToWebP(ms, cancellationToken);

            string imagePath = $"/img/{_area}/{fileName.ToDevName()}.webp";

            string finalImagePath = await _blobStore.SaveAsync(applicationTenantId, imagePath, webpStream, false, cancellationToken);

            await _imageCropsGenerator.CreateCropsAsync(applicationTenantId, finalImagePath, true, cancellationToken);

            var repo = _dataStore.GetRepository<WebContent>();

            var entity = await repo.FirstOrDefaultAsync(new WebContentFromIdAndTenantSpecification(applicationTenantId, eventId), cancellationToken);

            if (entity != null)
            {
                entity.SetImageUrl(finalImagePath.Replace(".webp", ""));
                await _dataStore.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

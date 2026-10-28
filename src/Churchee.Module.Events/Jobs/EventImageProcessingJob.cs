using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using Hangfire;

namespace Churchee.Module.Events.Jobs
{
    public class EventImageProcessingJob
    {
        private readonly IDataStore _dataStore;
        private readonly IBlobStore _blobStore;
        private readonly IImageProcessor _imageProcessor;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ImageCropsGenerator _imageCropsGenerator;

        public EventImageProcessingJob(IDataStore dataStore, IBlobStore blobStore, IImageProcessor imageProcessor, IBackgroundJobClient backgroundJobClient)
        {
            _dataStore = dataStore;
            _blobStore = blobStore;
            _imageProcessor = imageProcessor;
            _backgroundJobClient = backgroundJobClient;
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

            string imagePath = $"/img/events/{fileName.ToDevName()}.webp";

            string finalImagePath = await _blobStore.SaveAsync(applicationTenantId, imagePath, webpStream, false, cancellationToken);

            await _imageCropsGenerator.CreateCropsAsync(applicationTenantId, finalImagePath, true, cancellationToken);

            var repo = _dataStore.GetRepository<Event>();

            var entity = await repo.FirstOrDefaultAsync(new EventByIdAndTenantSpecification(applicationTenantId, eventId), cancellationToken);

            if (entity != null)
            {
                entity.SetImageUrl(finalImagePath);
                entity.Publish();
                await _dataStore.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;

namespace Churchee.ImageProcessing.Jobs
{
    public class ImageCropsGenerator
    {

        private readonly IBlobStore _blobStore;
        private readonly IImageProcessor _imageProcessor;

        public ImageCropsGenerator(IBlobStore blobStore, IImageProcessor imageProcessor)
        {
            _blobStore = blobStore;
            _imageProcessor = imageProcessor;
        }

        public async Task CreateCropsAsync(Guid applicationTenantId, string originalImagePath, bool overrideExisting, CancellationToken cancellationToken)
        {
            await CreateImageSizesAsync(applicationTenantId, originalImagePath, "t", 200, overrideExisting, cancellationToken);
            await CreateImageSizesAsync(applicationTenantId, originalImagePath, "s", 576, overrideExisting, cancellationToken);
            await CreateImageSizesAsync(applicationTenantId, originalImagePath, "m", 768, overrideExisting, cancellationToken);
            await CreateImageSizesAsync(applicationTenantId, originalImagePath, "l", 992, overrideExisting, cancellationToken);
            await CreateImageSizesAsync(applicationTenantId, originalImagePath, "xl", 1200, overrideExisting, cancellationToken);
            await CreateImageSizesAsync(applicationTenantId, originalImagePath, "xxl", 1400, overrideExisting, cancellationToken);
            await CreateImageSizesAsync(applicationTenantId, originalImagePath, "hd", 1920, overrideExisting, cancellationToken);

            await CreateImageCropsAsync(applicationTenantId, originalImagePath, "ct", 200, overrideExisting, cancellationToken);
            await CreateImageCropsAsync(applicationTenantId, originalImagePath, "cs", 576, overrideExisting, cancellationToken);
            await CreateImageCropsAsync(applicationTenantId, originalImagePath, "cm", 768, overrideExisting, cancellationToken);
            await CreateImageCropsAsync(applicationTenantId, originalImagePath, "cl", 992, overrideExisting, cancellationToken);
            await CreateImageCropsAsync(applicationTenantId, originalImagePath, "cxl", 1200, overrideExisting, cancellationToken);
            await CreateImageCropsAsync(applicationTenantId, originalImagePath, "cxxl", 1400, overrideExisting, cancellationToken);
            await CreateImageCropsAsync(applicationTenantId, originalImagePath, "chd", 1920, overrideExisting, cancellationToken);
        }

        private async Task CreateImageSizesAsync(Guid applicationTenantId, string originalImagePath, string suffix, int width, bool overrideExisting, CancellationToken cancellationToken)
        {
            var (fileName, folderPath) = GetFileInfo(originalImagePath);

            string imagePath = $"{folderPath}{fileName}_{suffix}";

            await CreateImageSizeAsync(applicationTenantId, originalImagePath, width, overrideExisting, imagePath, ".webp", cancellationToken);
        }

        private async Task CreateImageSizeAsync(Guid applicationTenantId, string originalImagePath, int width, bool overrideExisting, string imagePath, string extension, CancellationToken cancellationToken)
        {
            await using var stream = await _blobStore.GetReadStreamAsync(applicationTenantId, originalImagePath, cancellationToken);

            using var imageStream = await _imageProcessor.ResizeImageAsync(stream, width, 0, extension, cancellationToken);

            await _blobStore.SaveAsync(applicationTenantId, $"{imagePath}{extension}", imageStream, overrideExisting, cancellationToken);
        }

        private static (string fileName, string folderPath) GetFileInfo(string originalImagePath)
        {
            string extension = Path.GetExtension(originalImagePath);

            string fileName = Path.GetFileNameWithoutExtension(originalImagePath);

            string folderPath = originalImagePath.Replace(extension, string.Empty).Replace(fileName, string.Empty);

            return (fileName, folderPath);
        }

        private async Task CreateImageCropsAsync(Guid applicationTenantId, string originalImagePath, string suffix, int width, bool overrideExisting, CancellationToken cancellationToken)
        {
            var (fileName, folderPath) = GetFileInfo(originalImagePath);

            string imagePath = $"{folderPath}{fileName}_{suffix}";

            await CreateImageCropAsync(applicationTenantId, originalImagePath, width, overrideExisting, imagePath, ".webp", cancellationToken);
        }

        private async Task CreateImageCropAsync(Guid applicationTenantId, string originalImagePath, int width, bool overrideExisting, string imagePath, string extension, CancellationToken cancellationToken)
        {
            await using var stream = await _blobStore.GetReadStreamAsync(applicationTenantId, originalImagePath, cancellationToken);

            using var imageStream = await _imageProcessor.CreateCropAsync(stream, width, extension, cancellationToken);

            await _blobStore.SaveAsync(applicationTenantId, $"{imagePath}{extension}", imageStream, overrideExisting, default);
        }
    }
}

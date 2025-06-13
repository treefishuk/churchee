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

        public async Task CreateCrops(Guid applicationTenantId, string originalImagePath, byte[] streamBytes, bool overrideExisting)
        {
            await CreateImageSizes(applicationTenantId, originalImagePath, "t", streamBytes, 200, overrideExisting);
            await CreateImageSizes(applicationTenantId, originalImagePath, "s", streamBytes, 576, overrideExisting);
            await CreateImageSizes(applicationTenantId, originalImagePath, "m", streamBytes, 768, overrideExisting);
            await CreateImageSizes(applicationTenantId, originalImagePath, "l", streamBytes, 992, overrideExisting);
            await CreateImageSizes(applicationTenantId, originalImagePath, "xl", streamBytes, 1200, overrideExisting);
            await CreateImageSizes(applicationTenantId, originalImagePath, "xxl", streamBytes, 1400, overrideExisting);
            await CreateImageSizes(applicationTenantId, originalImagePath, "hd", streamBytes, 1920, overrideExisting);

            await CreateImageCrops(applicationTenantId, originalImagePath, "ct", streamBytes, 200, overrideExisting);
            await CreateImageCrops(applicationTenantId, originalImagePath, "cs", streamBytes, 576, overrideExisting);
            await CreateImageCrops(applicationTenantId, originalImagePath, "cm", streamBytes, 768, overrideExisting);
            await CreateImageCrops(applicationTenantId, originalImagePath, "cl", streamBytes, 992, overrideExisting);
            await CreateImageCrops(applicationTenantId, originalImagePath, "cxl", streamBytes, 1200, overrideExisting);
            await CreateImageCrops(applicationTenantId, originalImagePath, "cxxl", streamBytes, 1400, overrideExisting);
            await CreateImageCrops(applicationTenantId, originalImagePath, "chd", streamBytes, 1920, overrideExisting);
        }

        private async Task CreateImageSizes(Guid applicationTenantId, string originalImagePath, string suffix, byte[] streamBytes, int width, bool overrideExisting)
        {
            var (fileName, extension, folderPath) = GetFileInfo(originalImagePath);

            string imagePath = $"{folderPath}{fileName}_{suffix}";

            await CreateImageSize(applicationTenantId, suffix, streamBytes, width, overrideExisting, imagePath, extension);

            await CreateImageSize(applicationTenantId, suffix, streamBytes, width, overrideExisting, imagePath, ".webp");
        }

        private async Task CreateImageSize(Guid applicationTenantId, string suffix, byte[] streamBytes, int width, bool overrideExisting, string imagePath, string extension)
        {
            using var stream = new MemoryStream(streamBytes);

            var imageStream = _imageProcessor.ResizeImage(stream, width, 0, extension);

            await _blobStore.SaveAsync(applicationTenantId, $"{imagePath}{extension}", imageStream, overrideExisting, default);
        }

        private static (string fileName, string extension, string folderPath) GetFileInfo(string originalImagePath)
        {
            string extension = Path.GetExtension(originalImagePath);

            string fileName = Path.GetFileNameWithoutExtension(originalImagePath);

            string folderPath = originalImagePath.Replace(extension, string.Empty).Replace(fileName, string.Empty);

            return (fileName, extension, folderPath);
        }

        private async Task CreateImageCrops(Guid applicationTenantId, string originalImagePath, string suffix, byte[] streamBytes, int width, bool overrideExisting)
        {
            var (fileName, extension, folderPath) = GetFileInfo(originalImagePath);

            string imagePath = $"{folderPath}{fileName}_{suffix}";

            await CreateImageCrop(applicationTenantId, suffix, streamBytes, width, overrideExisting, imagePath, extension);

            await CreateImageCrop(applicationTenantId, suffix, streamBytes, width, overrideExisting, imagePath, ".webp");
        }

        private async Task CreateImageCrop(Guid applicationTenantId, string suffix, byte[] streamBytes, int width, bool overrideExisting, string imagePath, string extension)
        {
            using var stream = new MemoryStream(streamBytes);

            var smallImageStream = _imageProcessor.CreateCrop(stream, width, extension);

            await _blobStore.SaveAsync(applicationTenantId, $"{imagePath}{extension}", smallImageStream, overrideExisting, default);
        }
    }
}

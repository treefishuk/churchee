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
            await CreateImageSize(applicationTenantId, originalImagePath, "t", streamBytes, 200, overrideExisting);
            await CreateImageSize(applicationTenantId, originalImagePath, "s", streamBytes, 576, overrideExisting);
            await CreateImageSize(applicationTenantId, originalImagePath, "m", streamBytes, 768, overrideExisting);
            await CreateImageSize(applicationTenantId, originalImagePath, "l", streamBytes, 992, overrideExisting);
            await CreateImageSize(applicationTenantId, originalImagePath, "xl", streamBytes, 1200, overrideExisting);
            await CreateImageSize(applicationTenantId, originalImagePath, "xxl", streamBytes, 1400, overrideExisting);
            await CreateImageSize(applicationTenantId, originalImagePath, "hd", streamBytes, 1920, overrideExisting);

            await CreateImageCrop(applicationTenantId, originalImagePath, "ct", streamBytes, 200, overrideExisting);
            await CreateImageCrop(applicationTenantId, originalImagePath, "cs", streamBytes, 576, overrideExisting);
            await CreateImageCrop(applicationTenantId, originalImagePath, "cm", streamBytes, 768, overrideExisting);
            await CreateImageCrop(applicationTenantId, originalImagePath, "cl", streamBytes, 992, overrideExisting);
            await CreateImageCrop(applicationTenantId, originalImagePath, "cxl", streamBytes, 1200, overrideExisting);
            await CreateImageCrop(applicationTenantId, originalImagePath, "cxxl", streamBytes, 1400, overrideExisting);
            await CreateImageCrop(applicationTenantId, originalImagePath, "chd", streamBytes, 1920, overrideExisting);
        }

        private async Task CreateImageSize(Guid applicationTenantId, string originalImagePath, string suffix, byte[] streamBytes, int width, bool overrideExisting)
        {
            var (fileName, extension, folderPath) = GetFileInfo(originalImagePath);

            using var stream = new MemoryStream(streamBytes);

            string cropPath = $"{folderPath}{fileName}_{suffix}{extension}";

            var smallImageStream = _imageProcessor.ResizeImage(stream, width, 0, extension);

            await _blobStore.SaveAsync(applicationTenantId, cropPath, smallImageStream, overrideExisting, default);
        }

        private static (string fileName, string extension, string folderPath) GetFileInfo(string originalImagePath)
        {
            string extension = Path.GetExtension(originalImagePath);

            string fileName = Path.GetFileNameWithoutExtension(originalImagePath);

            string folderPath = originalImagePath.Replace(extension, string.Empty).Replace(fileName, string.Empty);

            return (fileName, extension, folderPath);
        }

        private async Task CreateImageCrop(Guid applicationTenantId, string originalImagePath, string suffix, byte[] streamBytes, int width, bool overrideExisting)
        {
            var (fileName, extension, folderPath) = GetFileInfo(originalImagePath);

            using var stream = new MemoryStream(streamBytes);

            string cropPath = $"{folderPath}{fileName}_{suffix}{extension}";

            var smallImageStream = _imageProcessor.CreateCrop(stream, width, extension);

            await _blobStore.SaveAsync(applicationTenantId, cropPath, smallImageStream, overrideExisting, default);
        }
    }
}

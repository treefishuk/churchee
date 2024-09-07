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
            string extension = Path.GetExtension(originalImagePath);

            string fileName = Path.GetFileNameWithoutExtension(originalImagePath);

            string folderPath = originalImagePath.Replace(extension, "").Replace(fileName, "");

            await CreateImageSize(applicationTenantId, fileName, folderPath, extension, "t", streamBytes, 200, overrideExisting);
            await CreateImageSize(applicationTenantId, fileName, folderPath, extension, "s", streamBytes, 576, overrideExisting);
            await CreateImageSize(applicationTenantId, fileName, folderPath, extension, "m", streamBytes, 768, overrideExisting);
            await CreateImageSize(applicationTenantId, fileName, folderPath, extension, "l", streamBytes, 992, overrideExisting);
            await CreateImageSize(applicationTenantId, fileName, folderPath, extension, "xl", streamBytes, 1200, overrideExisting);
            await CreateImageSize(applicationTenantId, fileName, folderPath, extension, "xxl", streamBytes, 1400, overrideExisting);
            await CreateImageSize(applicationTenantId, fileName, folderPath, extension, "hd", streamBytes, 1920, overrideExisting);

            await CreateImageCrop(applicationTenantId, fileName, folderPath, extension, "ct", streamBytes, 200, overrideExisting);
            await CreateImageCrop(applicationTenantId, fileName, folderPath, extension, "cs", streamBytes, 576, overrideExisting);
            await CreateImageCrop(applicationTenantId, fileName, folderPath, extension, "cm", streamBytes, 768, overrideExisting);
            await CreateImageCrop(applicationTenantId, fileName, folderPath, extension, "cl", streamBytes, 992, overrideExisting);
            await CreateImageCrop(applicationTenantId, fileName, folderPath, extension, "cxl", streamBytes, 1200, overrideExisting);
            await CreateImageCrop(applicationTenantId, fileName, folderPath, extension, "cxxl", streamBytes, 1400, overrideExisting);
            await CreateImageCrop(applicationTenantId, fileName, folderPath, extension, "chd", streamBytes, 1920, overrideExisting);
        }

        private async Task CreateImageSize(Guid applicationTenantId, string fileName, string folderPath, string extension, string suffix, byte[] streamBytes, int width, bool overrideExisting)
        {
            using var stream = new MemoryStream(streamBytes);

            string cropPath = $"{folderPath}{fileName}_{suffix}{extension}";

            var smallImageStream = _imageProcessor.ResizeImage(stream, width, 0, extension);

            await _blobStore.SaveAsync(applicationTenantId, cropPath, smallImageStream, overrideExisting, default);
        }

        private async Task CreateImageCrop(Guid applicationTenantId, string fileName, string folderPath, string extension, string suffix, byte[] streamBytes, int width, bool overrideExisting)
        {

            using var stream = new MemoryStream(streamBytes);

            string cropPath = $"{folderPath}{fileName}_{suffix}{extension}";

            var smallImageStream = _imageProcessor.CreateCrop(stream, width, extension);

            await _blobStore.SaveAsync(applicationTenantId, cropPath, smallImageStream, overrideExisting, default);
        }
    }
}

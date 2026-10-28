using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

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
            await using var sourceStream = await _blobStore.GetReadStreamAsync(applicationTenantId, originalImagePath, cancellationToken);
            using var sourceImage = await Image.LoadAsync(sourceStream, cancellationToken);

            var sizeTasks = new[]
            {
                CreateImageSizesAsync(applicationTenantId, originalImagePath, sourceImage, "t", 200, overrideExisting, cancellationToken),
                CreateImageSizesAsync(applicationTenantId, originalImagePath, sourceImage, "s", 576, overrideExisting, cancellationToken),
                CreateImageSizesAsync(applicationTenantId, originalImagePath, sourceImage, "m", 768, overrideExisting, cancellationToken),
                CreateImageSizesAsync(applicationTenantId, originalImagePath, sourceImage, "l", 992, overrideExisting, cancellationToken),
                CreateImageSizesAsync(applicationTenantId, originalImagePath, sourceImage, "xl", 1200, overrideExisting, cancellationToken),
                CreateImageSizesAsync(applicationTenantId, originalImagePath, sourceImage, "xxl", 1400, overrideExisting, cancellationToken),
                CreateImageSizesAsync(applicationTenantId, originalImagePath, sourceImage, "hd", 1920, overrideExisting, cancellationToken),
            };

            await Task.WhenAll(sizeTasks);

            var cropTasks = new[]
            {
                CreateImageCropsAsync(applicationTenantId, originalImagePath, sourceImage, "ct", 200, overrideExisting, cancellationToken),
                CreateImageCropsAsync(applicationTenantId, originalImagePath, sourceImage, "cs", 576, overrideExisting, cancellationToken),
                CreateImageCropsAsync(applicationTenantId, originalImagePath, sourceImage, "cm", 768, overrideExisting, cancellationToken),
                CreateImageCropsAsync(applicationTenantId, originalImagePath, sourceImage, "cl", 992, overrideExisting, cancellationToken),
                CreateImageCropsAsync(applicationTenantId, originalImagePath, sourceImage, "cxl", 1200, overrideExisting, cancellationToken),
                CreateImageCropsAsync(applicationTenantId, originalImagePath, sourceImage, "cxxl", 1400, overrideExisting, cancellationToken),
                CreateImageCropsAsync(applicationTenantId, originalImagePath, sourceImage, "chd", 1920, overrideExisting, cancellationToken),
            };

            await Task.WhenAll(cropTasks);
        }

        private async Task CreateImageSizesAsync(Guid applicationTenantId, string originalImagePath, Image sourceImage, string suffix, int width, bool overrideExisting, CancellationToken cancellationToken)
        {
            var (fileName, folderPath) = GetFileInfo(originalImagePath);

            string imagePath = $"{folderPath}{fileName}_{suffix}";

            await CreateImageSizeAsync(applicationTenantId, sourceImage, width, overrideExisting, imagePath, ".webp", cancellationToken);
        }

        private async Task CreateImageSizeAsync(Guid applicationTenantId, Image sourceImage, int width, bool overrideExisting, string imagePath, string extension, CancellationToken cancellationToken)
        {
            using var resizedImage = sourceImage.Clone(image => image.Resize(new ResizeOptions
            {
                Size = new Size(width, 0),
                Mode = ResizeMode.Max,
            }));

            await using var outputStream = new MemoryStream();
            await resizedImage.SaveAsync(outputStream, new WebpEncoder
            {
                FileFormat = WebpFileFormatType.Lossy,
                Quality = 70,
                Method = WebpEncodingMethod.Fastest,
                NearLossless = false,
            }, cancellationToken);

            outputStream.Position = 0;

            await _blobStore.SaveAsync(applicationTenantId, $"{imagePath}{extension}", outputStream, overrideExisting, cancellationToken);
        }

        private static (string fileName, string folderPath) GetFileInfo(string originalImagePath)
        {
            string extension = Path.GetExtension(originalImagePath);

            string fileName = Path.GetFileNameWithoutExtension(originalImagePath);

            string folderPath = originalImagePath.Replace(extension, string.Empty).Replace(fileName, string.Empty);

            return (fileName, folderPath);
        }

        private async Task CreateImageCropsAsync(Guid applicationTenantId, string originalImagePath, Image sourceImage, string suffix, int width, bool overrideExisting, CancellationToken cancellationToken)
        {
            var (fileName, folderPath) = GetFileInfo(originalImagePath);

            string imagePath = $"{folderPath}{fileName}_{suffix}";

            await CreateImageCropAsync(applicationTenantId, sourceImage, width, overrideExisting, imagePath, ".webp", cancellationToken);
        }

        private async Task CreateImageCropAsync(Guid applicationTenantId, Image sourceImage, int width, bool overrideExisting, string imagePath, string extension, CancellationToken cancellationToken)
        {
            var originalWidth = sourceImage.Width;
            var cropWidth = Math.Min(width, originalWidth);
            var cropX = (originalWidth - cropWidth) / 2;

            using var croppedImage = sourceImage.Clone(image =>
            {
                image.Crop(new Rectangle(cropX, 0, cropWidth, sourceImage.Height));
            });

            await using var outputStream = new MemoryStream();
            await croppedImage.SaveAsync(outputStream, new WebpEncoder
            {
                FileFormat = WebpFileFormatType.Lossy,
                Quality = 70,
                Method = WebpEncodingMethod.Fastest,
                NearLossless = false,
            }, cancellationToken);

            outputStream.Position = 0;

            await _blobStore.SaveAsync(applicationTenantId, $"{imagePath}{extension}", outputStream, overrideExisting, cancellationToken);
        }
    }
}

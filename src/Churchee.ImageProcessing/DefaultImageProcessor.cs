using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Churchee.ImageProcessing
{
    public class DefaultImageProcessor : IImageProcessor
    {
        private readonly IBlobStore _blobStore;
        private readonly ILogger _logger;

        public DefaultImageProcessor(IBlobStore blobStore, ILogger<DefaultImageProcessor> logger)
        {
            _blobStore = blobStore;
            _logger = logger;
        }

        private static Stream CreateCrop(Stream stream, int width, string extension)
        {
            var image = Image.Load(stream);

            int height = image.Height;

            stream.Position = 0;

            return Process(stream, width, height, extension);
        }

        private static Stream ResizeImage(Stream stream, int width, int height, string extension)
        {
            return Process(stream, width, height, extension);
        }

        private static Stream Process(Stream stream, int width, int height, string extension)
        {
            string normalizedExtension = extension.ToUpperInvariant();

            var encoder = SetEncoder(normalizedExtension);

            return Process(stream, width, height, encoder);
        }


        private static Stream Process(Stream stream, int width, int height, ImageEncoder imageEncoder)
        {
            var image = Image.Load(stream);

            if (width == 0 && height == 0)
            {
                return stream;
            }

            if (height == 0)
            {
                ProportionalMutation(width, height, image);
            }

            if (height != 0)
            {
                CropMutation(width, image);
            }

            var returnStream = new MemoryStream();

            //This saves to the memoryStream with encoder
            image.Save(returnStream, imageEncoder);

            returnStream.Position = 0;

            return returnStream;
        }

        private static ImageEncoder SetEncoder(string normalizedExtension)
        {
            if (normalizedExtension == ".JPG" || normalizedExtension == ".JPEG")
            {
                //Encode here for quality
                return new JpegEncoder()
                {
                    Quality = 100
                };
            }
            if (normalizedExtension == ".WEBP")
            {
                return new WebpEncoder()
                {
                    FileFormat = WebpFileFormatType.Lossy,
                    Quality = 75,
                    Method = WebpEncodingMethod.BestQuality,
                    FilterStrength = 60,
                    NearLossless = false,
                    NearLosslessQuality = 60
                };
            }
            return new PngEncoder()
            {
                CompressionLevel = PngCompressionLevel.BestCompression,
                BitDepth = PngBitDepth.Bit8,
                FilterMethod = PngFilterMethod.Adaptive,
                TransparentColorMode = PngTransparentColorMode.Preserve
            };
        }

        private static void ProportionalMutation(int width, int height, Image image)
        {
            image.Mutate(x => x.Resize(width, height));
        }

        private static void CropMutation(int width, Image image)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Ensure the crop width is not greater than the original width
            if (width > originalWidth)
            {
                width = originalWidth;
            }

            int cropX = (originalWidth - width) / 2;

            // Ensure the crop rectangle is within the image bounds
            if (cropX < 0)
            {
                cropX = 0;
            }

            image.Mutate(x => x.Crop(new Rectangle(cropX, 0, width, originalHeight)));
        }

        public async Task<Stream> ResizeImageAsync(Stream stream, int width, int height, string extension, CancellationToken cancellationToken)
        {
            return await Task.Run(() => ResizeImage(stream, width, height, extension), cancellationToken);
        }

        public async Task<Stream> CreateCropAsync(Stream stream, int width, string extension, CancellationToken cancellationToken)
        {
            return await Task.Run(() => CreateCrop(stream, width, extension), cancellationToken);
        }

        public async Task<Stream> ConvertToWebP(Stream stream, CancellationToken cancellationToken)
        {
            var imageInfo = await Image.IdentifyAsync(stream, cancellationToken);

            int width = imageInfo.Width;

            if (width > 1920)
            {
                width = 1920;
            }

            stream.Position = 0;

            var fullQualityEncoder = new WebpEncoder()
            {
                FileFormat = WebpFileFormatType.Lossless,
                Quality = 100,
            };

            return await Task.Run(() => Process(stream, width, 0, fullQualityEncoder));
        }

        public async Task<string> ConvertTempImageToFullImage(string path, string fileName, string folderName, Guid applicationTenantId, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName))
            {
                return string.Empty;
            }

            try
            {

                // Open the temp file stream (do not rely on await-using to delay disposal until method exit)
                var tempFileStream = File.OpenRead(path);

                using var webPStream = await ConvertToWebP(tempFileStream, cancellationToken);

                string imagePath = Path.Combine(folderName, $"{Path.GetFileNameWithoutExtension(fileName).ToDevName()}.webp");

                string webPPath = await _blobStore.SaveAsync(applicationTenantId, imagePath, webPStream, false, cancellationToken);

                // Ensure the file handle is released before attempting to delete the temp file
                await tempFileStream.DisposeAsync();

                return imagePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting temp image to full image for file {FileName} in folder {FolderName}", fileName, folderName);
            }
            finally
            {
                // Delete the temp file if it exists
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            return string.Empty;
        }
    }
}
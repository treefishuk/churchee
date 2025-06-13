using Churchee.Common.Abstractions.Utilities;
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

        public Stream CreateCrop(Stream stream, int width, string extension)
        {
            var image = Image.Load(stream);

            int height = image.Height;

            stream.Position = 0;

            return Process(stream, width, height, extension);
        }

        public Stream ResizeImage(Stream stream, int width, int height, string extension)
        {
            return Process(stream, width, height, extension);
        }

        private static Stream Process(Stream stream, int width, int height, string extension)
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

            string normalizedExtension = extension.ToUpperInvariant();

            var encoder = SetEncoder(normalizedExtension);

            //This saves to the memoryStream with encoder
            image.Save(returnStream, encoder);

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

    }
}
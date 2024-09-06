using Churchee.Common.Abstractions.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Churchee.ImageProcessing
{
    public class DefaultImageProcessor : IImageProcessor
    {
        public Stream ResizeImage(Stream stream, int width, int height, string extension)
        {
            var image = Image.Load(stream);

            if (width == 0 && height == 0)
            {
                return stream;
            }

            image.Mutate(x => x.Resize(width, height));

            var returnStream = new MemoryStream();

            ImageEncoder encoder;

            string normalizedExtension = extension.ToUpperInvariant();

            if (normalizedExtension == ".JPG" || normalizedExtension == ".JPEG")
            {
                //Encode here for quality
                encoder = new JpegEncoder()
                {
                    Quality = 100
                };
            }
            else
            {
                encoder = new PngEncoder()
                {
                    CompressionLevel = PngCompressionLevel.BestCompression,
                    BitDepth = PngBitDepth.Bit8,
                    FilterMethod = PngFilterMethod.Adaptive,
                    TransparentColorMode = PngTransparentColorMode.Preserve
                };
            }

            //This saves to the memoryStream with encoder
            image.Save(returnStream, encoder);

            returnStream.Position = 0;

            return returnStream;
        }
    }
}

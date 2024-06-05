using Churchee.Common.Abstractions.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Churchee.ImageProcessing
{
    public class DefaultImageProcessor : IImageProcessor
    {
        public Stream ResizeImage(Stream stream, int width, int height)
        {
            var image = Image.Load(stream);

            if (width == 0 && height == 0)
            {
                return stream;
            }

            image.Mutate(x => x.Resize(width, height));

            var returnStream = new MemoryStream();

            //Encode here for quality
            var encoder = new JpegEncoder()
            {
                Quality = 100
            };

            //This saves to the memoryStream with encoder
            image.Save(returnStream, encoder);

            returnStream.Position = 0;

            return returnStream;
        }
    }
}

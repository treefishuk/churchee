using System.IO;

namespace Churchee.Common.Abstractions.Utilities
{
    public interface IImageProcessor
    {
        /// <summary>
        /// Resize an Image
        /// </summary>
        /// <param name="stream">Source Stream</param>
        /// <param name="width">Use 0 to keep aspect raitio relative to height</param>
        /// <param name="height">Use 0 to keep aspect raitio relative to height</param>
        /// <returns>Resized Image Stream</returns>
        Stream ResizeImage(Stream stream, int width, int height, string extension);

        Stream CreateCrop(Stream stream, int width, string extension);
    }
}

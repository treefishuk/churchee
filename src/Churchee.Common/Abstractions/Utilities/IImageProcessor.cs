using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Utilities
{
    public interface IImageProcessor
    {
        /// <summary>
        /// Create a resized version of an image
        /// </summary>
        /// <param name="stream">Source Stream</param>
        /// <param name="width">Use 0 to keep aspect ratio relative to height</param>
        /// <param name="height">Use 0 to keep aspect ratio relative to width</param>
        /// <param name="extension">.png, .jpg, .webp etc.</param>
        /// <param name="cancellationToken">Pass through a cancelation token</param>
        /// <returns>Resized Image Stream</returns>
        Task<Stream> ResizeImageAsync(Stream stream, int width, int height, string extension, CancellationToken cancellationToken);

        /// <summary>
        /// Create a cropped version of an image
        /// </summary>
        /// <param name="stream">Source Stream</param>
        /// <param name="width">With of the desired crop</param>
        /// <param name="extension">.png, .jpg, .webp etc.</param>
        /// <param name="cancellationToken">Pass through a cancelation token</param>
        /// <returns>Cropped Image Stream</returns>
        Task<Stream> CreateCropAsync(Stream stream, int width, string extension, CancellationToken cancellationToken);

        /// <summary>
        /// Convert any image to webp format, resizing if necessary
        /// </summary>
        /// <param name="stream">Source Stream</param>
        /// <param name="cancellationToken">Pass through a cancelation token</param>
        /// <returns>WebP Image Stream</returns>
        Task<Stream> ConvertToWebP(Stream stream, CancellationToken cancellationToken);
    }
}

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.Text.RegularExpressions;

namespace Churchee.ImageProcessing.Validation
{
    public static class ImageValidation
    {
        public static bool BeAValidImageExtension(string extension)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
            return Array.Exists(validExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        public static bool BeValidImage(string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image))
            {
                return false;
            }

            try
            {
                var base64Pattern = @"^[a-zA-Z0-9\+/]*={0,2}$";

                string base64Trimmed = base64Image.Split(',')[1];

                if (!Regex.IsMatch(base64Trimmed, base64Pattern))
                {
                    return false;
                }

                var imageData = Convert.FromBase64String(base64Trimmed);

                using var ms = new MemoryStream(imageData);

                var image = Image.Load(ms);

                return image != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool BeExpectedFormat(string base64Image, string extension)
        {
            if (string.IsNullOrEmpty(base64Image) || string.IsNullOrEmpty(extension))
            {
                return false;
            }

            try
            {
                string base64Trimmed = base64Image.Split(',')[1];

                var imageData = Convert.FromBase64String(base64Trimmed);

                var format = Image.DetectFormat(imageData);

                var expectedFormat = GetImageFormatFromExtension(extension);

                return format == expectedFormat;
            }
            catch
            {
                return false;
            }
        }

        private static IImageFormat GetImageFormatFromExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentNullException(nameof(extension));
            }

            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance,
                ".png" => SixLabors.ImageSharp.Formats.Png.PngFormat.Instance,
                ".gif" => SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance,
                ".bmp" => SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance,
                _ => null,
            };
        }

    }

}
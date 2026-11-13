using System.Collections.Immutable;

namespace Churchee.Common.Validation
{
    public static class FileValidation
    {
        public static ImmutableArray<string> ImageFormats { get; } = [".jpg", ".jpeg", ".png", ".webp"];

        public static ImmutableArray<string> AllowedFormats { get; } = [".jpg", ".jpeg", ".png", ".webp", ".mp3", ".pdf", ".mp4"];

        public static bool BeValidPdf(string base64File)
        {
            return HasFileSignature(base64File, 0, "%PDF-"u8);
        }

        public static bool BeValidMp3(string base64File)
        {
            return HasFileSignature(base64File, 0, "ID3"u8) || HasMp3FrameSync(base64File);
        }

        private static bool HasMp3FrameSync(string? base64File)
        {
            if (string.IsNullOrWhiteSpace(base64File))
            {
                return false;
            }

            int commaIndex = base64File.IndexOf(',', StringComparison.InvariantCultureIgnoreCase);
            string base64 = commaIndex >= 0
                ? base64File[(commaIndex + 1)..]
                : base64File;

            try
            {
                byte[] bytes = Convert.FromBase64String(base64);

                return bytes.Length >= 2 &&
                       bytes[0] == 0xFF &&
                       (bytes[1] & 0xE0) == 0xE0;
            }
            catch (FormatException)
            {
                return false;
            }
        }


        public static bool BeValidMp4(string base64File)
        {
            return HasFileSignature(base64File, 4, "ftyp"u8);
        }

        public static bool IsImageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            // Ensure filePath is not just an extension (e.g., ".jpg")
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            string extension = Path.GetExtension(filePath)?.ToLowerInvariant();
            return ImageFormats.Contains(extension, StringComparer.InvariantCultureIgnoreCase);
        }


        private static bool HasFileSignature(
            string base64File,
            int offset,
            ReadOnlySpan<byte> signature)
        {
            if (string.IsNullOrWhiteSpace(base64File))
            {
                return false;
            }

            // Remove data URI prefix if present
            int commaIndex = base64File.IndexOf(',', StringComparison.InvariantCultureIgnoreCase);
            string base64 = commaIndex >= 0
                ? base64File[(commaIndex + 1)..]
                : base64File;

            try
            {
                byte[] bytes = Convert.FromBase64String(base64);

                return bytes.Length >= offset + signature.Length &&
                       bytes.AsSpan(offset, signature.Length).SequenceEqual(signature);
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }
}

using System;
using System.Collections.Immutable;
using System.IO;

namespace Churchee.Common.Validation
{
    public static class FileValidation
    {
        public static ImmutableArray<string> ImageFormats { get; } = [".jpg", ".jpeg", ".png", ".webp"];

        public static ImmutableArray<string> AllowedFormats { get; } = [".jpg", ".jpeg", ".png", ".webp", ".mp3", ".pdf", ".mp4"];

        public static bool BeValidPdf(string base64File)
        {
            if (string.IsNullOrWhiteSpace(base64File))
            {
                return false;
            }

            try
            {
                // Remove data URI prefix if present
                string[] parts = base64File.Split(',');
                string base64 = parts.Length > 1 ? parts[1] : parts[0];
                byte[] bytes = Convert.FromBase64String(base64);

                // PDF files start with "%PDF-"
                string pdfHeader = System.Text.Encoding.ASCII.GetString(bytes, 0, Math.Min(5, bytes.Length));
                return pdfHeader == "%PDF-";
            }
            catch
            {
                return false;
            }
        }

        public static bool BeValidMp3(string base64File)
        {
            if (string.IsNullOrWhiteSpace(base64File))
            {
                return false;
            }

            try
            {
                string[] parts = base64File.Split(',');
                string base64 = parts.Length > 1 ? parts[1] : parts[0];
                byte[] bytes = Convert.FromBase64String(base64);

                // Check for "ID3" at the start
                if (bytes.Length >= 3)
                {
                    string header = System.Text.Encoding.ASCII.GetString(bytes, 0, 3);
                    if (header == "ID3")
                    {
                        return true;
                    }
                }

                // Check for frame sync (0xFF 0xFB)
                return bytes.Length >= 2 && bytes[0] == 0xFF && (bytes[1] & 0xE0) == 0xE0;
            }
            catch
            {
                return false;
            }
        }

        public static bool BeValidMp4(string base64File)
        {
            if (string.IsNullOrWhiteSpace(base64File))
            {
                return false;
            }

            try
            {
                string[] parts = base64File.Split(',');
                string base64 = parts.Length > 1 ? parts[1] : parts[0];
                byte[] bytes = Convert.FromBase64String(base64);

                // "ftyp" is usually at offset 4
                if (bytes.Length >= 8)
                {
                    string ftyp = System.Text.Encoding.ASCII.GetString(bytes, 4, 4);
                    if (ftyp == "ftyp")
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
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
            return ImageFormats.Contains(extension);
        }
    }
}

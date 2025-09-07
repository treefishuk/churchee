using Churchee.Common.Validation;

namespace Churchee.Common.Tests.Validation
{
    public class FileValidationTests
    {
        [Theory]
        [InlineData("data:application/pdf;base64,JVBERi0xLjQKJcfsj6IK", true)] // "%PDF-" header
        [InlineData("JVBERi0xLjQKJcfsj6IK", true)] // "%PDF-" header, no prefix
        [InlineData("data:application/pdf;base64,SGVsbG8gd29ybGQ=", false)] // Not a PDF
        [InlineData("", false)]
        public void BeValidPdf_ReturnsExpected(string base64, bool expected)
        {
            Assert.Equal(expected, FileValidation.BeValidPdf(base64));
        }

        [Theory]
        [InlineData("data:audio/mp3;base64,/+MY", true)] // Frame sync 0xFF 0xE0
        [InlineData("/+MY", true)] // Frame sync 0xFF 0xE0, no prefix
        [InlineData("SGVsbG8gd29ybGQ=", false)] // Not an MP3
        [InlineData("", false)]
        public void BeValidMp3_ReturnsExpected(string base64, bool expected)
        {
            Assert.Equal(expected, FileValidation.BeValidMp3(base64));
        }

        [Theory]
        [InlineData("data:video/mp4;base64,AAAAFGZ0eXBtcDQy", true)] // "ftyp" at offset 4
        [InlineData("AAAAFGZ0eXBtcDQy", true)] // "ftyp" at offset 4, no prefix
        [InlineData("SGVsbG8gd29ybGQ=", false)] // Not an MP4
        [InlineData("", false)]
        public void BeValidMp4_ReturnsExpected(string base64, bool expected)
        {
            Assert.Equal(expected, FileValidation.BeValidMp4(base64));
        }

        [Theory]
        [InlineData("test.jpg", true)]
        [InlineData("test.jpeg", true)]
        [InlineData("test.png", true)]
        [InlineData("test.webp", true)]
        [InlineData("test.pdf", false)]
        [InlineData("test", false)]
        [InlineData("", false)]
        public void IsImageFile_ReturnsExpected(string filePath, bool expected)
        {
            Assert.Equal(expected, FileValidation.IsImageFile(filePath));
        }
    }

}

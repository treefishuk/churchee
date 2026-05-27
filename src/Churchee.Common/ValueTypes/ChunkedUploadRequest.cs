namespace Churchee.Common.ValueTypes
{
    public record ChunkedUploadRequest
    {
        public string FileName { get; set; }

        public string TempFilePath { get; set; }

        public string FinalFilePath { get; set; }
    }
}

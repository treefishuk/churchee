namespace System.IO
{
    public static class StreamExtensions
    {
        public static byte[] ConvertStreamToByteArray(this Stream stream)
        {
            stream.Position = 0;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}

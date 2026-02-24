using System.Security.Cryptography;

namespace Churchee.Module.Facebook.Events.Helpers
{
    public static class Hasher
    {
        public static async Task<string> HashFirst64KbAsync(Stream stream, CancellationToken cancellationToken)
        {
            const int MaxBytes = 64 * 1024; // 64 KB

            using var sha = SHA256.Create();

            byte[] buffer = new byte[8192];
            int totalRead = 0;

            while (totalRead < MaxBytes)
            {
                int read = await stream.ReadAsync(buffer.AsMemory(0, Math.Min(buffer.Length, MaxBytes - totalRead)), cancellationToken);
                if (read == 0)
                {
                    break;
                }

                sha.TransformBlock(buffer, 0, read, null, 0);
                totalRead += read;
            }

            sha.TransformFinalBlock([], 0, 0);

            stream.Position = 0; // Reset stream position after hashing

            return Convert.ToHexString(sha.Hash!);
        }
    }
}

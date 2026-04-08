namespace Churchee.Common.Tests.Helpers
{
    using global::Churchee.Common.Helpers;
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    namespace Churchee.Common.Tests.Helpers
    {
        public class HasherTests
        {
            [Fact]
            public async Task HashFirst64KbAsync_StreamLessThan64Kb_ReturnsHashOfWholeStreamAndResetsPosition()
            {
                byte[] data = Enumerable.Range(0, 1000).Select(i => (byte)(i % 256)).ToArray();
                using var ms = new MemoryStream(data);
                var ct = CancellationToken.None;

                // expected: SHA256 of the whole data
                string expected;
                using (var sha = SHA256.Create())
                {
                    expected = Convert.ToHexString(sha.ComputeHash(data));
                }

                string result = await Hasher.HashFirst64KbAsync(ms, ct);

                Assert.Equal(expected, result);
                Assert.Equal(0, ms.Position);
            }

            [Fact]
            public async Task HashFirst64KbAsync_StreamMoreThan64Kb_ReturnsHashOfFirst64KbAndResetsPosition()
            {
                int total = 100 * 1024; // 100 KB
                byte[] data = new byte[total];
                for (int i = 0; i < total; i++)
                {
                    data[i] = (byte)(i % 256);
                }

                using var ms = new MemoryStream(data);
                var ct = CancellationToken.None;

                int max = 64 * 1024;
                byte[] first = new byte[max];
                Array.Copy(data, 0, first, 0, max);

                // expected: SHA256 of the first 64 KB
                string expected;
                using (var sha = SHA256.Create())
                {
                    expected = Convert.ToHexString(sha.ComputeHash(first));
                }

                string result = await Hasher.HashFirst64KbAsync(ms, ct);

                Assert.Equal(expected, result);
                Assert.Equal(0, ms.Position);
            }

            [Fact]
            public async Task HashFirst64KbAsync_EmptyStream_ReturnsHashOfEmptyAndResetsPosition()
            {
                using var ms = new MemoryStream(Array.Empty<byte>());
                var ct = CancellationToken.None;

                string expected;
                using (var sha = SHA256.Create())
                {
                    expected = Convert.ToHexString(sha.ComputeHash(Array.Empty<byte>()));
                }

                string result = await Hasher.HashFirst64KbAsync(ms, ct);

                Assert.Equal(expected, result);
                Assert.Equal(0, ms.Position);
            }

            [Fact]
            public async Task HashFirst64KbAsync_WhenCancelled_ThrowsOperationCanceledExceptionAndResetsPosition()
            {
                byte[] data = Enumerable.Range(0, 10000).Select(i => (byte)(i % 256)).ToArray();
                using var ms = new MemoryStream(data);
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Assert.ThrowsAsync<OperationCanceledException>(() => Hasher.HashFirst64KbAsync(ms, cts.Token));

                Assert.Equal(0, ms.Position);
            }
        }
    }
}

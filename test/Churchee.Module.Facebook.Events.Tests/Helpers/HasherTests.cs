using Churchee.Module.Facebook.Events.Helpers;
using Churchee.Test.Helpers.Validation;
using System.Text;

namespace Churchee.Module.Facebook.Events.Tests.Helpers
{
    public class HasherTests
    {
        [Fact]
        public async Task Hashing_Match_Test()
        {
            using var stream = StringToStream("ABC123");

            string hash1 = await Hasher.HashFirst64KbAsync(stream, CancellationToken.None);

            hash1.Should().Be("E0BEBD22819993425814866B62701E2919EA26F1370499C1037B53B9D49C2C8A");
        }

        [Fact]
        public async Task Hashing_Fail_Test()
        {
            using var stream = StringToStream("123ABC");

            string hash1 = await Hasher.HashFirst64KbAsync(stream, CancellationToken.None);

            hash1.Should().NotBe("E0BEBD22819993425814866B62701E2919EA26F1370499C1037B53B9D49C2C8A");
        }


        private static MemoryStream StringToStream(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return new MemoryStream(bytes);
        }

    }
}

using Churchee.Test.Helpers.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Common.Tests.Extensions
{
    public class StreamExtensionsTests
    {
        [Fact]
        public void StreamExtensionsTests_ConvertStreamToByteArray_ReturnsByteArray()
        {
            //arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Test"));

            //act
            var bytes = stream.ConvertStreamToByteArray();
            string byteString = System.Text.Encoding.Default.GetString(bytes);

            bytes.Should().NotBeNull();
            byteString.Should().Be("Test");

        }
    }
}

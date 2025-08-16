using Churchee.Common.Exceptions;
using FluentAssertions;

namespace Churchee.Common.Tests.Exceptions
{
    public class MissingConfirgurationSettingExceptionTests
    {

        [Fact]
        public void MissingConfirgurationSettingException_WithEmptyConstructor_SetsPropertiesCorrectly()
        {
            //arrange & act
            var cut = new MissingConfigurationSettingException();

            //assert
            cut.Message.Should().Be("A configuration setting is missing");

        }

        [Fact]
        public void MissingConfirgurationSettingException_WithMessage_SetsMessage()
        {
            //arrange & act
            var cut = new MissingConfigurationSettingException("Test");

            //assert
            cut.Message.Should().Be("Test");
        }

        [Fact]
        public void MissingConfirgurationSettingException_WithMessageAndInnerException_SetsMessageAndInnerException()
        {
            //arrange
            var innerException = new InvalidOperationException("Invalid Opperation");

            //act
            var cut = new MissingConfigurationSettingException("Test", innerException);

            //assert
            cut.Message.Should().Be("Test");
            cut.InnerException.Should().NotBeNull();
            cut.InnerException?.Message.Should().Be("Invalid Opperation");
        }

    }
}

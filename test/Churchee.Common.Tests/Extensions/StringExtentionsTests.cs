using FluentAssertions;

namespace Churchee.Common.Tests.Extensions
{
    public class StringExtentionsTests
    {
        [Fact]
        public void StringExtentions_ToTitleCase_ReturnsTitleCase()
        {
            //arrange
            string sut = "Do a thing";

            //act
            var result = sut.ToTitleCase();

            //assert
            result.Should().Be("Do A Thing");
        }


        [Fact]
        public void StringExtentions_DoAThingToSentence_ReturnsSentance()
        {
            //arrange
            string sut = "DoThatThing";

            //act
            var result = sut.ToSentence();

            //assert
            result.Should().Be("Do That Thing");
        }

        [Fact]
        public void StringExtentions_ToCamelCase_ReturnsCamelCase()
        {
            //arrange
            string sut = "Do A Thing";

            //act
            var result = sut.ToCamelCase();

            //assert
            result.Should().Be("doAThing");
        }

        [Fact]
        public void StringExtentions_ToCamelCase_ReturnsEmpty_WhenEmpty()
        {
            //arrange
            string sut = "";

            //act
            var result = sut.ToCamelCase();

            //assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void StringExtentions_ToCamelCase_ReturnsNull_WhenNull()
        {
            //arrange
            string? sut = null;

            //act
            var result = sut.ToCamelCase();

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public void StringExtentions_ToPascalCase_ReturnsCamelCase()
        {
            //arrange
            string sut = "do a thing";

            //act
            var result = sut.ToPascalCase();

            //assert
            result.Should().Be("DoAThing");
        }

        [Fact]
        public void StringExtentions_ToPascalCase_ReturnsEmpty_WhenEmpty()
        {
            //arrange
            string sut = "";

            //act
            var result = sut.ToPascalCase();

            //assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void StringExtentions_ToPascalCase_ReturnsNull_WhenNull()
        {
            //arrange
            string? sut = null;

            //act
            var result = sut.ToPascalCase();

            //assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("1", "1st")]
        [InlineData("21", "21st")]
        [InlineData("31", "31st")]
        [InlineData("2", "2nd")]
        [InlineData("22", "22nd")]
        [InlineData("3", "3rd")]
        [InlineData("23", "23rd")]
        [InlineData("5", "5th")]
        [InlineData("", "")]
        public void StringExtentions_AddSuffix_ReturnsExpected(string input, string expected)
        {
            //act
            var result = input.AddSuffix();

            //assert
            result.Should().Be(expected);
        }
    }
}
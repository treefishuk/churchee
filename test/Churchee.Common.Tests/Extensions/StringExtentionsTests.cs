using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void StringExtentions_ToPascalCase_ReturnsCamelCase()
        {
            //arrange
            string sut = "do a thing";

            //act
            var result = sut.ToPascalCase();

            //assert
            result.Should().Be("DoAThing");
        }

    }
}

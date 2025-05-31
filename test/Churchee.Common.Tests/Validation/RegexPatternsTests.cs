using Bogus;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Churchee.Common.Tests.Validation
{
    public partial class RegexPatternsTests
    {
        private readonly Faker _faker;

        public RegexPatternsTests()
        {
            _faker = new Faker();
        }

        [GeneratedRegex(RegexPattern.Name, RegexOptions.ExplicitCapture)]
        private static partial Regex NameRegex();

        [Fact]
        public void RegexPattern_Name_GivenValidName_Passes()
        {
            //arrange
            string input = _faker.Name.FullName();

            //act
            var match = NameRegex().IsMatch(input);

            //assert
            match.Should().BeTrue();
        }

        [GeneratedRegex(RegexPattern.Phone, RegexOptions.ExplicitCapture)]
        private static partial Regex PhoneRegex();

        [Fact]
        public void RegexPattern_Phone_GivenValidLandline_Passes()
        {
            //arrange
            string input = _faker.Phone.PhoneNumber("01#########");

            //act
            var match = PhoneRegex().IsMatch(input);

            //assert
            match.Should().BeTrue();
        }

        [Fact]
        public void RegexPattern_Phone_GivenValidMobile_Passes()
        {
            //arrange
            string input = _faker.Phone.PhoneNumber("07#########");

            //act
            var match = PhoneRegex().IsMatch(input);

            //assert
            match.Should().BeTrue();
        }

        [GeneratedRegex(RegexPattern.SingleLowercaseWord, RegexOptions.ExplicitCapture)]
        private static partial Regex SingleLowercaseWordRegex();

        [Fact]
        public void RegexPattern_SingleLowercaseWord_GivenValidString_Passes()
        {
            //arrange
            string input = "test";

            //act
            var match = SingleLowercaseWordRegex().IsMatch(input);

            //assert
            match.Should().BeTrue();
        }


        [Fact]
        public void RegexPattern_SingleLowercaseWord_GivenInValidString_Fails()
        {
            //arrange
            string input = "Test";

            //act
            var match = SingleLowercaseWordRegex().IsMatch(input);

            //assert
            match.Should().BeFalse();
        }

        [GeneratedRegex(RegexPattern.BasicText, RegexOptions.ExplicitCapture)]
        private static partial Regex BasicTextRegex();

        [Fact]
        public void RegexPattern_BasicText_GivenValidString_IsTrue()
        {
            //arrange
            string input = "I'm a basic string";

            //act
            var match = BasicTextRegex().IsMatch(input);

            //assert
            match.Should().BeTrue();
        }

        [Fact]
        public void RegexPattern_BasicText_GivenInValidString_IsFalse()
        {
            //arrange
            string input = "I-am-invalid!";

            //act
            var match = BasicTextRegex().IsMatch(input);

            //assert
            match.Should().BeFalse();
        }
    }
}

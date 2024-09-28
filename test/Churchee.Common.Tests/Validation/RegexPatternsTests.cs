using Bogus;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Churchee.Common.Tests.Validation
{
    public class RegexPatternsTests
    {
        private readonly Faker _faker;

        public RegexPatternsTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void RegexPattern_Name_GivenValidName_Passes()
        {
            //arrange
            string input = _faker.Name.FullName();

            //act
            var match = Regex.Match(input, RegexPattern.Name, RegexOptions.IgnoreCase);

            //assert
            match.Success.Should().BeTrue();
        }

        [Fact]
        public void RegexPattern_Phone_GivenValidLandline_Passes()
        {
            //arrange
            string phoneNumber = _faker.Phone.PhoneNumber("01#########");

            //act
            var match = Regex.Match(phoneNumber, RegexPattern.Phone, RegexOptions.IgnoreCase);

            //assert
            match.Success.Should().BeTrue();
        }

        [Fact]
        public void RegexPattern_Phone_GivenValidMobile_Passes()
        {
            //arrange
            string phoneNumber = _faker.Phone.PhoneNumber("07#########");

            //act
            var match = Regex.Match(phoneNumber, RegexPattern.Phone, RegexOptions.IgnoreCase);

            //assert
            match.Success.Should().BeTrue();
        }

        [Fact]
        public void RegexPattern_SingleLowercaseWord_GivenValidString_Passes()
        {
            //arrange
            string input = "test";

            //act
            var match = Regex.Match(input, RegexPattern.SingleLowercaseWord, RegexOptions.ExplicitCapture);

            //assert
            match.Success.Should().BeTrue();
        }

        [Fact]
        public void RegexPattern_SingleLowercaseWord_GivenInValidString_Fails()
        {
            //arrange
            string input = "Test";

            //act
            var match = Regex.Match(input, RegexPattern.SingleLowercaseWord, RegexOptions.ExplicitCapture);

            //assert
            match.Success.Should().BeFalse();
        }

        [Fact]
        public void RegexPattern_BasicText_GivenValidString_IsTrue()
        {
            //arrange
            string input = "I'm a basic string";

            //act
            var match = Regex.Match(input, RegexPattern.BasicText, RegexOptions.ExplicitCapture);

            //assert
            match.Success.Should().BeTrue();
        }

        [Fact]
        public void RegexPattern_BasicText_GivenInValidString_IsFalse()
        {
            //arrange
            string input = "I-am-invalid!";

            //act
            var match = Regex.Match(input, RegexPattern.BasicText, RegexOptions.ExplicitCapture);

            //assert
            match.Success.Should().BeFalse();
        }
    }
}

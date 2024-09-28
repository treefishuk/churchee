using Churchee.Common.ResponseTypes;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Common.Tests.ResponseTypes
{
    public class ValidationErrorTests
    {
        [Fact]
        public void ValidationError_PropertiesGetSet()
        {
            var cut = new ValidationError("Error", "Code");

            cut.Description.Should().Be("Error");
            cut.Property.Should().Be("Code");
        }

    }
}

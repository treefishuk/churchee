using Churchee.Common.ValueTypes;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Common.Tests.ValueTypes
{
    public class MultiSelectItemTests
    {
        [Fact]
        public void MultiSelectItem_TwoArgumentsProvided_PropertiesSetCorrectly()
        {
            //arrange
            var value = Guid.NewGuid();

            //act
            var cut = new MultiSelectItem(value, "Test");

            //assert
            cut.Text.Should().Be("Test");
            cut.Value.Should().Be(value);
            cut.Selected.Should().BeFalse();
        }

        [Fact]
        public void MultiSelectItem_ThreeArgumentsProvided_PropertiesSetCorrectly()
        {
            //arrange
            var value = Guid.NewGuid();

            //act
            var cut = new MultiSelectItem(value, "Test", true);

            //assert
            cut.Text.Should().Be("Test");
            cut.Value.Should().Be(value);
            cut.Selected.Should().BeTrue();
        }
    }
}

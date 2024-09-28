using Churchee.Common.ValueTypes;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Common.Tests.ValueTypes
{
    public class MultiSelectTests
    {
        [Fact]
        public void MultiSelect_PropertiesSetCorrectly()
        {
            var items = new List<MultiSelectItem>
            {
                new MultiSelectItem(Guid.Empty, "Test", true),
                new MultiSelectItem(Guid.Empty, "Test 2"),
            };

            var cut = new MultiSelect(items);

            cut.Items.Count().Should().Be(2);
            cut.SelectedValues.Count().Should().Be(1);

        }

    }
}

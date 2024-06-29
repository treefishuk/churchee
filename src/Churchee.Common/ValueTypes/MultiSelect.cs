using System;
using System.Collections.Generic;

namespace Churchee.Common.ValueTypes
{
    public class MultiSelect
    {
        public MultiSelect(IEnumerable<MultiSelectItem> items)
        {
            Items = items;
            SelectedValues = [];
        }

        public IEnumerable<MultiSelectItem> Items { get; set; }

        public IEnumerable<Guid> SelectedValues { get; set; }
    }
}

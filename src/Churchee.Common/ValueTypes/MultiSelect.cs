using System;
using System.Collections.Generic;
using System.Linq;

namespace Churchee.Common.ValueTypes
{
    public class MultiSelect
    {
        public MultiSelect(IEnumerable<MultiSelectItem> items)
        {
            Items = items;
            SelectedValues = items.Where(w => w.Selected).Select(s => s.Value).ToList();
        }

        public IEnumerable<MultiSelectItem> Items { get; set; }

        public IEnumerable<Guid> SelectedValues { get; set; }
    }
}

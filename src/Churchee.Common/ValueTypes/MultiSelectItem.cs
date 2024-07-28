using System;

namespace Churchee.Common.ValueTypes
{
    public class MultiSelectItem
    {
        public Guid Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
    }
}

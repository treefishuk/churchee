using System;

namespace Churchee.Common.ValueTypes
{
    public class MultiSelectItem
    {
        public MultiSelectItem(Guid value, string text, bool selected)
        {
            Value = value;
            Text = text;
            Selected = selected;
        }
        public MultiSelectItem(Guid value, string text) : this(value, text, false)
        {
        }

        public Guid Value { get; set; }

        public string Text { get; set; }

        public bool Selected { get; set; }
    }
}

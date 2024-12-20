namespace Churchee.Module.UI.Models
{
    public class DropdownInput
    {

        public DropdownInput()
        {
            Title = string.Empty;
            Value = string.Empty;
            Data = [];
        }

        public string Title { get; set; }

        public string Value { get; set; }

        public IEnumerable<DropdownInput> Data { get; set; }
    }
}

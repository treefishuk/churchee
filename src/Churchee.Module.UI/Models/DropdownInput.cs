namespace Churchee.Module.UI.Models
{
    public class DropdownInput
    {
        public string Title { get; set; }

        public string Value { get; set; }

        public IEnumerable<DropdownInput> Data { get; set; }
    }
}

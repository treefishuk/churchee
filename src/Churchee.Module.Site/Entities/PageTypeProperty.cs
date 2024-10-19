using Churchee.Common.Data;

namespace Churchee.Module.Site.Entities
{
    public class PageTypeProperty : Entity
    {
        public PageType PageType { get; set; }

        public string Name { get; set; }

        public string DevName => Name.ToDevName();

        public string Type { get; set; }

        public bool Required { get; set; }
    }
}

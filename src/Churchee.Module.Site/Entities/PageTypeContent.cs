using Churchee.Common.Data;

namespace Churchee.Module.Site.Entities
{
    public class PageTypeContent : Entity
    {
        private PageTypeContent()
        {

        }

        public PageTypeContent(Guid pageTypeContentId, Guid applicationTenantId, string type, bool isRequired, string name, int order) : base(pageTypeContentId, applicationTenantId)
        {
            Type = type;
            IsRequired = isRequired;
            Name = name;
            DevName = name.ToDevName();
            Order = order;
        }

        public string Name { get; private set; }

        public string DevName { get; private set; }

        public PageType PageType { get; internal set; }

        public string Type { get; private set; }

        public bool IsRequired { get; private set; }

        public int Order { get; set; }

        public void UpdateDetails(bool isRequired, string name, string type, int order)
        {

            IsRequired = isRequired;
            Name = name;
            Type = type;
            Order = order;
        }

    }
}

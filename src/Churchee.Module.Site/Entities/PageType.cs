using Churchee.Common.Data;
using Churchee.Module.Site.Events;

namespace Churchee.Module.Site.Entities
{
    public class PageType : AggregateRoot
    {
        private PageType()
        {

        }

        public PageType(Guid id, Guid systemKey, Guid applicationTenantId, bool allowInRoot, string name, bool triggerPageTypeCreatedEvent = true) : base(id, applicationTenantId)
        {
            SystemKey = systemKey;
            AllowInRoot = allowInRoot;
            Name = name;
            DevName = name.Replace(" ", "");
            ParentTypes = new List<PageTypeTypeMapping>();
            ChildrenTypes = new List<PageTypeTypeMapping>();
            PageTypeProperties = new List<PageTypeProperty>();
            PageTypeContent = new List<PageTypeContent>();
            Pages = new List<WebContent>();

            if (triggerPageTypeCreatedEvent)
            {
                AddDomainEvent(new PageTypeCreatedEvent(applicationTenantId, name));
            }

        }

        public Guid? SystemKey { get; private set; }

        public string Name { get; private set; }

        public string DevName { get; private set; }

        public bool AllowInRoot { get; private set; }

        public ICollection<WebContent> Pages { get; set; }

        public ICollection<PageTypeTypeMapping> ParentTypes { get; set; }

        public ICollection<PageTypeTypeMapping> ChildrenTypes { get; set; }

        public ICollection<PageTypeProperty> PageTypeProperties { get; set; }

        public ICollection<PageTypeContent> PageTypeContent { get; set; }


        public void AddPageTypeContent(Guid id, string name, string type, bool required)
        {
            var newPageTypeContent = new PageTypeContent(id, ApplicationTenantId, type, required, name);

            PageTypeContent.Add(newPageTypeContent);

            AddDomainEvent(new PageTypeContentCreatedEvent(ApplicationTenantId, newPageTypeContent.Id, Id));

        }

        public void AddChildType(PageType pageType)
        {
            ChildrenTypes.Add(new PageTypeTypeMapping { ParentPageType = this, ChildPageType = pageType });
        }

    }
}

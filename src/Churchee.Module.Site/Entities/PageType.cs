using Churchee.Common.Data;
using Churchee.Module.Site.Events;

namespace Churchee.Module.Site.Entities
{
    public class PageType : AggregateRoot
    {
        private PageType()
        {

        }

        public PageType(Guid id, Guid applicationTenantId, bool allowInRoot, string name, bool triggerPageTypeCreatedEvent = true) : base(id, applicationTenantId)
        {
            AllowInRoot = allowInRoot;
            Name = name;
            DevName = name.Replace(" ", "");
            ParentTypes = new List<WebContentTypeTypeMapping>();
            ChildrenTypes = new List<WebContentTypeTypeMapping>();
            PageTypeProperties = new List<WebContentTypeProperty>();
            PageTypeContent = new List<WebContentTypeContent>();
            Pages = new List<WebContent>();

            if (triggerPageTypeCreatedEvent)
            {
                AddDomainEvent(new PageTypeCreatedEvent(applicationTenantId, name));
            }

        }


        public string Name { get; private set; }

        public string DevName { get; private set; }

        public bool AllowInRoot { get; private set; }

        public ICollection<WebContent> Pages { get; set; }

        public ICollection<WebContentTypeTypeMapping> ParentTypes { get; set; }

        public ICollection<WebContentTypeTypeMapping> ChildrenTypes { get; set; }

        public ICollection<WebContentTypeProperty> PageTypeProperties { get; set; }

        public ICollection<WebContentTypeContent> PageTypeContent { get; set; }


        public void AddPageTypeContent(Guid id, string name, string type, bool required)
        {
            var newPageTypeContent = new WebContentTypeContent(id, ApplicationTenantId, type, required, name);

            PageTypeContent.Add(newPageTypeContent);

            AddDomainEvent(new PageTypeContentCreatedEvent(ApplicationTenantId, newPageTypeContent.Id, Id));

        }

    }
}

using Churchee.Module.Site.Events;

namespace Churchee.Module.Site.Entities
{
    public class Article : WebContent
    {
        private Article()
        {

        }

        public Article(Guid applicationTenantId, Guid pageTypeId, Guid parentId, string title, string url, string description) : base(applicationTenantId, title, url, description)
        {
            PageTypeId = pageTypeId;
            ParentId = parentId;
            IsSystem = true;
        }

        public string Content { get; private set; }

        public void UpdateInfo(string title, string description, Guid? parentId)
        {
            Title = title;
            Description = description;
            ParentId = parentId;

            AddDomainEvent(new PageInfoUpdatedEvent(Id));
        }

        public void SetContent(string content)
        {
            Content = content;
        }

        public void SetPublishDate(DateTime? publishOn)
        {
            LastPublishedDate = publishOn;
        }

        public void Publish()
        {
            LastPublishedDate = DateTime.Now;
            Published = true;
        }

        public void UnPublish()
        {
            Published = false;
        }
    }
}

using Churchee.Common.Abstractions.Entities;

namespace Churchee.Module.Site.Entities
{
    public class PageContent : IEntity
    {

        public PageContent()
        {
            Version = 0;
        }

        public PageContent(Guid pageTypeContentId, Guid pageId, string value, int version)
        {
            Version = version;
            PageTypeContentId = pageTypeContentId;
            Id = pageId;
            Value = value;
        }

        public Guid Id { get; set; }

        public Page Page { get; set; }

        public Guid PageTypeContentId { get; set; }

        public WebContentTypeContent PageTypeContent { get; set; }

        public int Version { get; private set; }

        public string Value { get; set; }

        public void IncrementVersion()
        {
            Version++;
        }

        public bool Deleted { get; set; }
    }
}

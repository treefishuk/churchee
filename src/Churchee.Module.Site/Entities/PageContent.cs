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
            PageId = pageId;
            Value = value;
        }

        public Guid PageId { get; set; }

        public Page Page { get; set; }

        public Guid PageTypeContentId { get; set; }

        public PageTypeContent PageTypeContent { get; set; }

        public int Version { get; private set; }

        public string Value { get; set; }

        public void IncrementVersion()
        {
            Version++;
        }

        public bool Deleted { get; set; }
    }
}

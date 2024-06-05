using Churchee.Module.Site.Helpers;

namespace Churchee.Module.Site.Entities
{
    public class Article : WebContent
    {
        private Article()
        {

        }

        public Article(Guid applicationTenantId, Guid parentId, string title, string url, string descrition) : base(applicationTenantId, title, url, descrition)
        {
            PageTypeId = PageTypes.BlogDetailPageTypeId;
            ParentId = parentId;
            IsSystem = true;
        }

        public string Content { get; private set; }

        public void SetContent(string content)
        {
            Content = content;
        }
    }
}

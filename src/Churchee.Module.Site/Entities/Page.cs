using Churchee.Module.Site.Events;
using System.Text.Json;

namespace Churchee.Module.Site.Entities
{
    public class Page : WebContent
    {
        protected Page()
        {
            PageContent = new List<PageContent>();
        }

        public Page(Guid applicationTenantId, string title, string url, string metadescription, Guid pageTypeId, Guid? parentId, bool triggerEvents) : base(applicationTenantId, title, url, metadescription)
        {
            PageTypeId = pageTypeId;

            Version = 0;

            PageContent = new List<PageContent>();

            ParentId = parentId;

            if (triggerEvents)
            {
                AddDomainEvent(new PageCreatedEvent(Id, pageTypeId));
            }

        }

        public int Version { get; private set; }

        public ICollection<PageContent> PageContent { get; set; }

        public void AddContent(Guid pageTypeContentId, Guid pageId, string content, int version)
        {
            var newContent = new PageContent(pageTypeContentId, pageId, content, version);

            PageContent.Add(newContent);
        }

        public void UpdateInfo(string title, string description, Guid? parentId)
        {
            Title = title;
            Description = description;
            ParentId = parentId;

            AddDomainEvent(new PageInfoUpdatedEvent(Id));
        }

        public void UpdateContent(List<KeyValuePair<Guid, string>> content)
        {

            foreach (var item in content)
            {
                PageContent.FirstOrDefault(d => d.PageTypeContentId == item.Key).Value = item.Value;
            }

        }

        public void Publish()
        {
            if (PageContent.Count != 0)
            {
                foreach (var content in PageContent)
                {
                    content.IncrementVersion();
                }
            }

            var versionData = new
            {
                Content = PageContent.Select(s => new { Name = s.PageTypeContent.DevName, s.Value }).ToList(),
            };

            PublishedData = JsonSerializer.Serialize(versionData, jsonSerializerOptions);

            LastPublishedDate = DateTime.Now;

            Published = true;
        }

        private static JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}

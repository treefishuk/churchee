using Churchee.Common.Data;

namespace Churchee.Module.Site.Entities
{
    public class ViewTemplate : Entity
    {
        private ViewTemplate()
        {

        }

        public ViewTemplate(Guid applicationTenantId, string location, string content) : base(applicationTenantId)
        {
            Location = location;
            Content = content;
        }

        public DateTime LastRequested { get; }

        public string Location { get; private set; }

        public string TenantLocation { get; }

        public string Content { get; private set; }

        public void SetContent(string content)
        {
            Content = content;
        }

    }
}

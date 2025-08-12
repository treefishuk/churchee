using Churchee.Common.Abstractions.Entities;

namespace Churchee.Module.Site.Entities
{
    public class RedirectUrl : IEntity
    {
        public RedirectUrl(string path)
        {
            Path = path;
        }

        public RedirectUrl(string path, Guid webContentId)
        {
            Path = path;
            WebContentId = webContentId;
        }

        public int Id { get; set; }

        public string Path { get; private set; }

        public bool Deleted { get; set; }

        public Guid WebContentId { get; set; }

        public virtual WebContent WebContent { get; set; }
    }
}

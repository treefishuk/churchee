using Churchee.Common.Data;

namespace Churchee.Module.Reviews.Entities
{
    public class Review : Entity
    {
        private Review()
        {

        }

        public Review(Guid applicationTenantId) : base(applicationTenantId)
        {

        }

        public int Rating { get; set; }

        public string ReviewerName { get; set; }

        public string ReviewerImageUrl { get; set; }

        public string Comment { get; set; }

        public string SourceName { get; set; }

        public string SourceId { get; set; }
    }
}

using Churchee.Common.Abstractions.Queries;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetListingQueryResponseItem : IHierarchicalDataTableResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public bool HasChildren { get; set; }

        public Guid? ParentId { get; set; }

        public bool Published { get; set; }

        [Display(Name = "Created/Modified")]
        [DataType(DataTypes.Date)]
        public DateTime? Modified { get; set; }

    }
}

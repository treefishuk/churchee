using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetListingQueryResponseItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }

        public bool HasChildren { get; set; }

        public Guid? ParentId { get; set; }

        public bool Published { get; set; }

    }
}

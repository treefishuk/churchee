using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Module.Site.Features.Pages.Queries.GetPageContent;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetPageDetailsResponse
    {
        public string Title { get; set; }

        public Guid? ParentId { get; set; }

        public string ParentName { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public IEnumerable<GetPageDetailsResponseContentItem> ContentItems { get; set; }



    }
}

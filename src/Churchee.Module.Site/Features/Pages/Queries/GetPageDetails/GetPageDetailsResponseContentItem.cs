using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Module.Site.Features.Pages.Queries.GetPageContent;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetPageDetailsResponseContentItem
    {
        public Guid PageTypeContentId { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public string DevName { get; set; }

        public string Value { get; set; }



    }
}

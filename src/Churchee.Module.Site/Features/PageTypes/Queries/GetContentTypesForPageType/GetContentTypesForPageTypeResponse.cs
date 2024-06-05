using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Site.Features.PageTypes.Queries
{
    public class GetContentTypesForPageTypeResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DevName { get; set; }

        public string Type { get; set; }

        public bool Required { get; set; }

        public int Order { get; set; }
    }
}

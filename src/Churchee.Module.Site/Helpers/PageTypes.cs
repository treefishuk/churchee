using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Site.Helpers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Fine")]
    public static class PageTypes
    {
        public static readonly Guid BlogListingPageTypeId = Guid.Parse("3c5c3620-f8a2-42ef-8a49-3e82912363cf");
        public static readonly Guid BlogDetailPageTypeId = Guid.Parse("f4c623c3-0047-4940-ad01-ca42db5ba54b");
        public static readonly Guid EventListingPageTypeId = Guid.Parse("f9c4c0cf-3908-4993-aa31-c59310ada766");
        public static readonly Guid EventDetailPageTypeId = Guid.Parse("1325d848-a18a-4b09-8d38-bdb1c94f885a");

    }
}

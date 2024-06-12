using System;
using System.Collections.Generic;
using System.Text;

namespace Churchee.Module.Site.Entities
{
    public class PageTypeTypeMapping
    {
        public Guid ParentPageTypeApplicationTenantId { get; set; }

        public Guid ParentPageTypeId { get; set; }

        public PageType ParentPageType { get; set; }

        public Guid ChildPageTypeId { get; set; }

        public Guid ChildPageTypeApplicationTenantId { get; set; }

        public PageType ChildPageType { get; set; }

    }
}

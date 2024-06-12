using System;
using System.Collections.Generic;
using System.Text;

namespace Churchee.Module.Site.Entities
{
    public class PageTypeTypeMapping
    {
        public Guid ParentPageTypeId { get; set; }

        public PageType ParentPageType { get; set; }

        public Guid ChildPageTypeId { get; set; }

        public PageType ChildPageType { get; set; }

    }
}

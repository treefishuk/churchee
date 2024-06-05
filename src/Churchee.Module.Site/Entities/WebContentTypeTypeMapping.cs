using System;
using System.Collections.Generic;
using System.Text;

namespace Churchee.Module.Site.Entities
{
    public class WebContentTypeTypeMapping
    {
        public Guid ParentWebContentTypeId { get; set; }

        public PageType ParentWebContentType { get; set; }

        public Guid ChildWebContentTypeId { get; set; }

        public PageType ChildWebContentType { get; set; }

    }
}

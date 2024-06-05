using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Churchee.Module.Site.Events
{
    public class PageCreatedEvent : INotification
    {
        public PageCreatedEvent(Guid pageId, Guid pageTypeId)
        {
            PageId = pageId;
            PageTypeId = pageTypeId;
        }

        public Guid PageId { get; set; }

        public Guid PageTypeId { get; set; }

    }
}

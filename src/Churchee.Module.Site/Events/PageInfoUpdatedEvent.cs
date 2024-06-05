using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Churchee.Module.Site.Events
{
    public class PageInfoUpdatedEvent : INotification
    {
        public PageInfoUpdatedEvent(Guid pageId)
        {
            PageId = pageId;
        }

        public Guid PageId { get; set; }

    }
}

using Churchee.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Events.Entities
{
    public class EventDate
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public virtual Event Event { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

    }
}

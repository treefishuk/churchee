using System;

namespace Churchee.Module.Logging.Features.Queries
{
    public class GetLogListingResponseItem
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string Level { get; set; }

        public DateTime TimeStamp { get; set; }

    }
}

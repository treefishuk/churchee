using System;

namespace Churchee.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime TruncateToSeconds(this DateTime dt)
        {
            return dt.AddTicks(-(dt.Ticks % TimeSpan.TicksPerSecond));
        }

    }
}

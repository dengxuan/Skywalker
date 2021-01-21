using System;
using System.Runtime.CompilerServices;

namespace Skywalker.Scheduler
{
    internal static class Utility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static DateTime Now(DateTime? timestamp = null)
        {
            return new DateTime(timestamp.HasValue ? timestamp.Value.Ticks : DateTime.Now.Ticks, DateTimeKind.Utc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TimeSpan GetDuration(DateTime timestamp)
        {
            return timestamp - (timestamp.Kind == DateTimeKind.Utc ? Now() : DateTime.Now);
        }
    }
}

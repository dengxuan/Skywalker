
using System;

namespace Skywalker.Scheduler
{
    public class OccurredEventArgs : EventArgs
    {
        public OccurredEventArgs(string scheduleId, int count)
        {
            this.Count = count;
            this.ScheduleId = scheduleId;
        }

        public int Count { get; }

        public string ScheduleId { get; }
    }
}

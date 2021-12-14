using System;

namespace Skywalker.Extensions.Timing
{
    public class SkywalkerClockOptions
    {
        /// <summary>
        /// Default: <see cref="DateTimeKind.Unspecified"/>
        /// </summary>
        public DateTimeKind Kind { get; set; }

        public SkywalkerClockOptions()
        {
            Kind = DateTimeKind.Unspecified;
        }
    }
}
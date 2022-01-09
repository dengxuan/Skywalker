using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Extensions.WheelTimer
{
    public class AtomicLong
    {
        private long _value;

        public long IncrementAndGet()
        {
            return Interlocked.Increment(ref _value);
        }

        public long DecrementAndGet()
        {
            return Interlocked.Decrement(ref _value);
        }

        public long Value => Interlocked.Read(ref _value);
    }
}

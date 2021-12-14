﻿using System.Threading;

namespace Skywalker.Extensions.WheelTimer
{
    public class AtomicInteger
    {
        private int _value;

        public int IncrementAndGet()
        {
            return Interlocked.Increment(ref _value);
        }

        public int DecrementAndGet()
        {
            return Interlocked.Decrement(ref _value);
        }
    }
}

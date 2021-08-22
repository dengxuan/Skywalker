using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Extensions.WheelTimer
{
    internal sealed class HashedWheelTimeout : IWheelTimeout
    {
        private volatile int _state;

        private readonly HashedWheelTimer _timer;

        public int State => _state;

        public long Deadline { get; private set; }

        // remainingRounds will be calculated and set by Worker.transferTimeoutsToBuckets() before the
        // HashedWheelTimeout will be added to the correct HashedWheelBucket.
        public long RemainingRounds { get; set; }

        public bool Expired => _state == TimeoutState.Expired;

        public bool Cancelled => _state == TimeoutState.Cancelled;

        // The bucket to which the timeout was added
        public HashedWheelBucket? Bucket { get; set; }

        // This will be used to chain timeouts in HashedWheelTimerBucket via a double-linked-list.
        // As only the workerThread will act on it there is no need for synchronization / volatile.
        public HashedWheelTimeout? Next { get; set; }

        public HashedWheelTimeout? Prev { get; set; }

        public IWheelTimer Timer => _timer;

        public IWheelTimerTask TimerTask { get; private set; }

        public HashedWheelTimeout(HashedWheelTimer timer, IWheelTimerTask timerTask, long deadline)
        {
            _timer = timer;
            TimerTask = timerTask;
            Deadline = deadline;
            Interlocked.Exchange(ref _state, TimeoutState.Init);
        }

        public void DecrementPendingTimeouts()
        {
            _timer.DecrementPendingTimeouts();
        }

        public bool Cancel()
        {
            // only update the state it will be removed from HashedWheelBucket on next tick.
            if (!CompareAndSetState(TimeoutState.Init, TimeoutState.Cancelled))
            {
                return false;
            }

            // If a task should be canceled we put this to another queue which will be processed on each tick.
            // So this means that we will have a GC latency of max. 1 tick duration which is good enough. This way
            // we can make again use of our MpscLinkedQueue and so minimize the locking / overhead as much as possible.
            _timer.EnqueueCanceledTimeout(this);
            return true;
        }

        public void Remove()
        {
            if (Bucket != null)
            {
                Bucket.Remove(this);
            }
            else
            {
                _timer.DecrementPendingTimeouts();
            }
        }

        public void Expire()
        {
            if (!CompareAndSetState(TimeoutState.Init, TimeoutState.Expired))
            {
                return;
            }

            Task.Factory.StartNew(async timeout =>
            {
                //
                await TimerTask.RunAsync((IWheelTimeout)timeout!);
            }, this).ConfigureAwait(false);
        }

        public override string ToString()
        {
            long currentTime = DateTimeHelper.TotalMilliseconds;
            long remaining = Deadline - currentTime + _timer.StartTime;

            var sb = new StringBuilder(192)
                .Append(GetType().Name)
                .Append('(')
                .Append("deadline: ");
            if (remaining > 0)
            {
                sb.Append(remaining)
                    .Append(" ms later");
            }
            else if (remaining < 0)
            {
                sb.Append(-remaining)
                    .Append(" ms ago");
            }
            else
            {
                sb.Append("now");
            }

            if (Cancelled)
            {
                sb.Append(", cancelled");
            }

            return sb.Append(", task: ")
                .Append(TimerTask)
                .Append(')')
                .ToString();
        }

        private bool CompareAndSetState(int expected, int state)
        {
            var originalState = Interlocked.CompareExchange(ref _state, state, expected);
            return originalState == expected;
        }
    }
}

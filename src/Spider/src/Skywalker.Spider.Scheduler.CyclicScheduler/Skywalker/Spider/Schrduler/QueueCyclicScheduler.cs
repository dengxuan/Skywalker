using Skywalker.Spider.DuplicateRemover.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Scheduler;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Spider.Schrduler
{
    public class QueueCyclicScheduler : SchedulerBase
    {
        private readonly ConcurrentQueue<Request> _requests = new();

        public QueueCyclicScheduler(IDuplicateRemover duplicateRemover, IRequestHasher requestHasher) : base(duplicateRemover, requestHasher)
        {
        }

        public override Task FailAsync(Request request)
        {
            return base.FailAsync(request);
        }

        public override Task<bool> IsEmpty()
        {
            return Task.FromResult(_requests.IsEmpty);
        }

        public override Task SuccessAsync(Request request)
        {
            return base.SuccessAsync(request);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _requests.Clear();
            }
        }

        protected override Task PushWhenNoDuplicate(Request request)
        {
            _requests.Enqueue(request);
            return Task.CompletedTask;
        }

        protected override Task<Request?> SafeDequeueAsync()
        {
            return Task.Run(() =>
            {
                if (_requests.TryDequeue(out Request? request))
                {
                    return request.Clone();
                }
                return null;
            });
        }
    }
}

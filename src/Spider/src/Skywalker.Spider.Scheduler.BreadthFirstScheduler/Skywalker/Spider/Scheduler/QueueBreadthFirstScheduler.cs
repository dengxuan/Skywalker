using Skywalker.Spider.DuplicateRemover.Abstractions;
using Skywalker.Spider.Http;

namespace Skywalker.Spider.Scheduler
{
    public class QueueBreadthFirstScheduler : SchedulerBase
    {
        private readonly List<Request> _requests = new();

        /// <summary>
        /// 构造方法
        /// </summary>
        public QueueBreadthFirstScheduler(IDuplicateRemover duplicateRemover, IRequestHasher requestHasher) : base(duplicateRemover, requestHasher)
        {
        }

        public override Task<bool> IsEmpty()
        {
            return Task.FromResult(_requests.IsNullOrEmpty());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _requests.Clear();
            }
        }

        /// <summary>
        /// 如果请求未重复就添加到队列中
        /// </summary>
        /// <param name="request">请求</param>
        protected override Task PushWhenNoDuplicate(Request request)
        {
            if (request == null)
            {
                return Task.CompletedTask;
            }

            _requests.Add(request);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 从队列中取出指定爬虫的指定个数请求
        /// </summary>
        /// <param name="count">出队数</param>
        /// <returns>请求</returns>
        protected override Task<Request?> SafeDequeueAsync()
        {
            var request = _requests.FirstOrDefault();
            if(request != null)
            {
                _requests.Remove(request);
            }
            return Task.FromResult(request?.Clone());
        }
    }
}

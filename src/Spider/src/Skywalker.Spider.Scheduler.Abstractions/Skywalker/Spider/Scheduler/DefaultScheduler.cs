using Skywalker.Spider.DuplicateRemover.Abstractions;
using Skywalker.Spider.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Spider.Scheduler
{
    internal class DefaultScheduler : SchedulerBase
    {
        private readonly ConcurrentQueue<Request> _requests = new();

        /// <summary>
        /// 构造方法
        /// </summary>
        public DefaultScheduler(IDuplicateRemover duplicateRemover, IRequestHasher requestHasher) : base(duplicateRemover, requestHasher)
        {
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

            _requests.Enqueue(request);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 从队列中取出指定爬虫的指定个数请求
        /// </summary>
        /// <param name="count">出队数</param>
        /// <returns>请求</returns>
        protected override Task<IEnumerable<Request>> SafeDequeueAsync(int count = 1)
        {
            return Task.Run(() =>
            {
                List<Request> result = new();
                for (int i = 0; i < count; i++)
                {
                    if (_requests.TryDequeue(out Request? request))
                    {
                        result.Add(request);
                    }
                }
                return result.Select(x => x.Clone());
            });
        }
    }
}

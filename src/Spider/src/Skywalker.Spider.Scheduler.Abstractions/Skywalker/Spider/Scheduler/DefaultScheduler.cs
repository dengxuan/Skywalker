using Skywalker.Spider.DuplicateRemover.Abstractions;
using Skywalker.Spider.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Skywalker.Spider.Scheduler;

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

    public override Task<bool> IsEmpty()
    {
        return Task.FromResult(_requests.IsEmpty);
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
    protected override Task<Request?> SafeDequeueAsync()
    {
        _requests.TryDequeue(out Request? request);
        return Task.FromResult(request?.Clone());
    }
}

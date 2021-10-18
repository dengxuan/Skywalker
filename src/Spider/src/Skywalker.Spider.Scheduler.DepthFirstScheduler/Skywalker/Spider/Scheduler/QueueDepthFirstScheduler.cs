using Skywalker.Spider.DuplicateRemover.Abstractions;
using Skywalker.Spider.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Spider.Scheduler;

public class QueueDepthFirstScheduler : SchedulerBase
{
    private readonly List<Request> _requests = new();

    /// <summary>
    /// 构造方法
    /// </summary>
    public QueueDepthFirstScheduler(IDuplicateRemover duplicateRemover, IRequestHasher requestHasher) : base(duplicateRemover, requestHasher)
    {
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
        var dequeueCount = 1;
        int start;
        if (_requests.Count < 1)
        {
            dequeueCount = _requests.Count;
            start = 0;
        }
        else
        {
            start = _requests.Count - dequeueCount - 1;
        }

        var requests = new List<Request>();
        for (var i = _requests.Count - 1; i >= start; --i)
        {
            requests.Add(_requests[i]);
        }

        if (dequeueCount > 0)
        {
            _requests.RemoveRange(start, dequeueCount);
        }

        return Task.FromResult(requests.Select(x => x.Clone()).FirstOrDefault());
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
        return Task.FromResult(_requests.IsNullOrEmpty());
    }
}

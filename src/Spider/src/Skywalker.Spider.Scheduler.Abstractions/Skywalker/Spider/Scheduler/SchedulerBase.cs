using Skywalker.Spider.DuplicateRemover.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Scheduler.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Spider.Scheduler;

public abstract class SchedulerBase : IScheduler
{
    //private SpinLock _spinLocker;
    private bool disposedValue;

    private readonly IRequestHasher _requestHasher;

    protected readonly IDuplicateRemover DuplicateRemover;

    protected SchedulerBase(IDuplicateRemover duplicateRemover, IRequestHasher requestHasher)
    {
        _requestHasher = requestHasher;
        DuplicateRemover = duplicateRemover;
    }

    /// <summary>
    /// 重置去重器
    /// </summary>
    public virtual async Task ResetDuplicateCheckAsync()
    {
        await DuplicateRemover.ResetDuplicateCheckAsync();
    }

    public abstract Task<bool> IsEmpty();

    public virtual Task SuccessAsync(Request request)
    {
        return Task.CompletedTask;
    }

    public virtual Task FailAsync(Request request)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 如果请求未重复就添加到队列中
    /// </summary>
    /// <param name="request">请求</param>
    protected abstract Task PushWhenNoDuplicate(Request request);

    /// <summary>
    /// 队列中的总请求个数
    /// </summary>
    public async Task<long> GetTotalAsync()
    {
        return await DuplicateRemover.GetTotalAsync();
    }

    /// <summary>
    /// 从队列中取出指定爬虫的指定个数请求
    /// </summary>
    /// <param name="count">出队数</param>
    /// <returns>请求</returns>
    protected abstract Task<Request?> SafeDequeueAsync();

    public async Task<Request?> DequeueAsync()
    {
        return await SafeDequeueAsync();
    }

    /// <summary>
    /// 请求入队
    /// </summary>
    /// <param name="spiderId"></param>
    /// <param name="requests">请求</param>
    /// <returns>入队个数</returns>
    public async Task<int> EnqueueAsync(IEnumerable<Request> requests)
    {
        var count = 0;
        foreach (var request in requests)
        {
            _requestHasher.ComputeHash(request);
            bool isDuplicate = await DuplicateRemover.IsDuplicateAsync(request);
            if (isDuplicate)
            {
                continue;
            }

            await PushWhenNoDuplicate(request);
            count++;
        }

        return count;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                DuplicateRemover.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    /// <summary>
    /// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    /// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    /// </summary>
    ~SchedulerBase()
    {
        Dispose(disposing: false);
    }
}

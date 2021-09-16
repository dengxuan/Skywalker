using Skywalker.Spider.DuplicateRemover.Abstractions;
using Skywalker.Spider.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.DuplicateRemover;

internal class NoneDuplicateRemover : IDuplicateRemover
{
    private long _counter;

    public NoneDuplicateRemover()
    {
        _counter = 0;
    }

    public void Dispose()
    {
    }

    public Task<bool> IsDuplicateAsync(Request request)
    {
        request.NotNull(nameof(request));
        Interlocked.Increment(ref _counter);
        return Task.FromResult(false);
    }

    public Task<long> GetTotalAsync()
    {
        return Task.FromResult(_counter);
    }

    public Task ResetDuplicateCheckAsync()
    {
        return Task.CompletedTask;
    }
}

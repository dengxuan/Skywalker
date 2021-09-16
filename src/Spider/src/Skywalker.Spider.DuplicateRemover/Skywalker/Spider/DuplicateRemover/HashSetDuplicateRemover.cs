using Skywalker.Spider.DuplicateRemover.Abstractions;
using Skywalker.Spider.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Spider.DuplicateRemover;

/// <summary>
/// 通过哈希去重
/// </summary>
public class HashSetDuplicateRemover : IDuplicateRemover
{
    private bool disposedValue = false;
    private readonly HashSet<string> _sets = new();

    /// <summary>
    /// Check whether the request is duplicate.
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Whether the request is duplicate.</returns>
    public Task<bool> IsDuplicateAsync(Request request)
    {
        return Task.Run(() =>
        {
            request.NotNull(nameof(request));

            lock (this)
            {
                bool isDuplicate = _sets.Add(request.Hash);
                return !isDuplicate;
            }
        });

    }

    public Task<long> GetTotalAsync()
    {
        return Task.Run(() =>
        {
            lock (this)
            {
                return (long)_sets.Count;
            }
        });
    }

    /// <summary>
    /// 重置去重器
    /// </summary>
    public Task ResetDuplicateCheckAsync()
    {
        return Task.Run(() =>
        {
            lock (this)
            {
                _sets.Clear();
            }
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _sets.Clear();
            }

            disposedValue = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///  Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ///  Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    /// </summary>
    ~HashSetDuplicateRemover()
    {
        Dispose(disposing: false);
    }
}

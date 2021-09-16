using Skywalker.Spider.Http;
using System;
using System.Threading.Tasks;

namespace Skywalker.Spider.DuplicateRemover.Abstractions;

public interface IDuplicateRemover : IDisposable
{
    /// <summary>
    /// Check whether the request is duplicate.
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Whether the request is duplicate.</returns>
    Task<bool> IsDuplicateAsync(Request request);

    /// <summary>
    /// 获取总数
    /// </summary>
    Task<long> GetTotalAsync();

    /// <summary>
    /// Reset duplicate check.
    /// </summary>
    Task ResetDuplicateCheckAsync();
}

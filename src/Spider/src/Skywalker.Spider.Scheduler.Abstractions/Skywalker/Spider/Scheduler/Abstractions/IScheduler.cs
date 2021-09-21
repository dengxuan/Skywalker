using Skywalker.Spider.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Spider.Scheduler.Abstractions
{
    /// <summary>
    /// 调度器接口
    /// </summary>
    public interface IScheduler : IDisposable
	{
		/// <summary>
		/// 从队列中取出请求
		/// </summary>
		/// <returns>请求</returns>
		Task<Request?> DequeueAsync();

		/// <summary>
		/// 请求入队
		/// </summary>
		/// <param name="requests">请求</param>
		/// <returns>入队个数</returns>
		Task<int> EnqueueAsync(IEnumerable<Request> requests);

		/// <summary>
		/// 队列中的总请求个数
		/// </summary>
		Task<long> GetTotalAsync();

		/// <summary>
		/// 队列是否空了
		/// </summary>
		/// <returns></returns>
		Task<bool> IsEmpty();

		/// <summary>
		/// 重置
		/// </summary>
		/// <returns></returns>
		Task ResetDuplicateCheckAsync();

		/// <summary>
		/// 标记请求成功
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task SuccessAsync(Request request);

		/// <summary>
		/// 标记请求失败
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task FailAsync(Request request);
	}
}

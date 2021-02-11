using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Queries.Abstractions
{
    public interface ISearcher
    {
        /// <summary>
        /// 异步发送无查询参数的查询到一个查询处理器
        /// </summary>
        /// <typeparam name="TOutput">查询结果</typeparam>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>一个发送操作的任务. 任务结果包含查询处理器处理的结果</returns>
        Task<TOutput> SearchAsync<TOutput>(CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步发送查询到一个查询Handler
        /// </summary>
        /// <typeparam name="TQuery">查询参数类型</typeparam>
        /// <typeparam name="TOutput">查询结果类型</typeparam>
        /// <param name="querier">查询参数</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>一个发送操作的任务. 任务结果包含查询处理器处理的结果</returns>
        Task<TOutput> SearchAsync<TQuery, TOutput>(TQuery querier, CancellationToken cancellationToken = default) where TQuery : IQuery<TOutput>;
    }
}

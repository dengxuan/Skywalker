using Skywalker.Application.Dtos.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Application.Abstractions
{
    public interface IApplication
    {
        /// <summary>
        /// 异步发送无查询参数的查询到一个查询处理器
        /// </summary>
        /// <typeparam name="TOutputDto">查询结果</typeparam>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>一个发送操作的任务. 任务结果包含查询处理器处理的结果</returns>
        Task<TOutputDto?> ExecuteAsync<TOutputDto>(CancellationToken cancellationToken = default) where TOutputDto : IEntityDto;

        /// <summary>
        /// 异步发送查询到一个查询Handler
        /// </summary>
        /// <typeparam name="TInputDto">查询参数类型</typeparam>
        /// <typeparam name="TOutputDto">查询结果类型</typeparam>
        /// <param name="inputDto">查询参数</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>一个发送操作的任务. 任务结果包含查询处理器处理的结果</returns>
        Task<TOutputDto?> ExecuteAsync<TInputDto, TOutputDto>(TInputDto inputDto, CancellationToken cancellationToken = default) where TInputDto : IEntityDto where TOutputDto : IEntityDto;
    }
}

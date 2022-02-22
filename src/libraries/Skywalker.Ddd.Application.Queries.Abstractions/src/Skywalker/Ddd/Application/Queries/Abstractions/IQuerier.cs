using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Queries.Abstractions;

public interface IQuerier
{
    /// <summary>
    /// 异步发送查询到一个查询Handler
    /// </summary>
    /// <typeparam name="TRequest">查询参数类型</typeparam>
    /// <typeparam name="TResponse">查询结果类型</typeparam>
    /// <param name="request">查询参数</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>一个发送操作的任务. 任务结果包含查询处理器处理的结果</returns>
    ValueTask<TResponse?> QueryAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto where TResponse : IResponseDto;
}

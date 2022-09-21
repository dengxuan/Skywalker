using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Application.Abstractions;

public interface IApplication : ISingletonDependency
{
    /// <summary>
    /// Asynchronously send a command to a single handler
    /// </summary>
    /// <typeparam name="TRequest">请求参数类型</typeparam>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>一个发送操作的任务</returns>
    ValueTask ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto;

    /// <summary>
    /// 异步发送查询到一个查询Handler
    /// </summary>
    /// <typeparam name="TRequest">请求参数类型</typeparam>
    /// <typeparam name="TResponse">查询结果类型</typeparam>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>一个发送操作的任务. 任务结果包含查询处理器处理的结果</returns>
    ValueTask<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto where TResponse : IResponseDto;
}

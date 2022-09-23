﻿using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Application.Abstractions;

/// <summary>
/// Send a request through the mediator pipeline to be handled by a single handler.
/// </summary>
public interface IApplication : ISingletonDependency
{
    ///// <summary>
    ///// 将请求异步发送到单个处理程序
    ///// </summary>
    ///// <typeparam name="TRequest"></typeparam>
    ///// <typeparam name="TResponse">响应类型</typeparam>
    ///// <param name="request">请求对象</param>
    ///// <param name="cancellationToken">可选的取消令牌</param>
    ///// <returns>表示发送操作的任务。 任务结果包含处理程序响应</returns>
    //ValueTask<TResponse> ExecuteAsync<TRequest, TResponse>(IRequestDto<TResponse> request, CancellationToken cancellationToken = default)
    //    where TRequest : IRequestDto<TResponse>
    //    where TResponse : notnull;

    ///// <summary>
    ///// 通过动态分派将对象请求异步发送到单个处理程序
    ///// </summary>
    ///// <typeparam name="THandler"></typeparam>
    ///// <param name="request">请求对象</param>
    ///// <param name="cancellationToken">可选的取消令牌</param>
    ///// <returns>一个发送操作的任务</returns>
    //ValueTask ExecuteAsync<THandler>(IRequestDto request, CancellationToken cancellationToken = default)
    //    where THandler : IApplicationHandler<IRequestDto>;

    /// <summary>
    /// 将命令异步发送到单个处理程序
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">可选的取消令牌</param>
    /// <returns>一个发送操作的任务</returns>
    ValueTask ExecuteAsync(IRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步发送查询到一个查询Handler
    /// </summary>
    /// <typeparam name="TResponse">查询结果类型</typeparam>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>一个发送操作的任务. 任务结果包含查询处理器处理的结果</returns>
    ValueTask<TResponse> ExecuteAsync<TResponse>(IRequestDto<TResponse> request, CancellationToken cancellationToken = default);
}

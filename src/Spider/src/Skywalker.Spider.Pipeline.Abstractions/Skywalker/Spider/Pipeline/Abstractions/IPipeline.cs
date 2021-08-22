using System;
using System.Threading.Tasks;

namespace Skywalker.Spider.Pipeline.Abstractions;

/// <summary>
/// 数据处理管道
/// </summary>
public interface IPipeline : IDisposable
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();

    /// <summary>
    /// 流处理
    /// </summary>
    /// <param name="context">处理上下文</param>
    /// <returns></returns>
    Task HandleAsync(PipelineContext context);
}

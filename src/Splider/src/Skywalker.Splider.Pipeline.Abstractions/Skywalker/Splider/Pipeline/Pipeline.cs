using Microsoft.Extensions.Logging;
using Skywalker.Splider.Pipeline.Abstractions;
using System;
using System.Threading.Tasks;

namespace Skywalker.Splider.Pipeline;

public abstract class Pipeline : IPipeline
{
    private bool disposedValue = false;

    protected ILogger<Pipeline> Logger { get; }

    public Pipeline(ILogger<Pipeline> logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    public abstract Task InitializeAsync();

    /// <summary>
    /// 流处理
    /// </summary>
    /// <param name="context">处理上下文</param>
    /// <returns></returns>
    public abstract Task HandleAsync(PipelineContext context);

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool IsNullOrEmpty(PipelineContext context)
    {
        return context.IsEmpty;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">if true, dispose managed state (managed objects). otherwise, free unmanaged resources (unmanaged objects) and override finalizer, set large fields to null.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            disposedValue = true;
        }
    }

    /// <summary>
    /// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    /// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    /// </summary>
    ~Pipeline()
    {
        Dispose(disposing: false);
    }
}


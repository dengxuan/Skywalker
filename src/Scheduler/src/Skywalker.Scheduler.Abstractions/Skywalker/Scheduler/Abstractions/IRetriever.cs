using System;

namespace Skywalker.Scheduler.Abstractions
{
    /// <summary>
    /// 提供调度失败的重试机制的接口。
    /// </summary>
    public interface IRetriever
    {
        /// <summary>表示重试丢弃的事件。</summary>
        event EventHandler<HandledEventArgs> Discarded;

        /// <summary>表示重试失败的事件。</summary>
        event EventHandler<HandledEventArgs> Failed;

        /// <summary>表示重试成功的事件。</summary>
        event EventHandler<HandledEventArgs> Succeed;

        /// <summary>
        /// 启动重试。
        /// </summary>
        void Run();

        /// <summary>
        /// 停止重试。
        /// </summary>
        /// <param name="clean">指定是否清空积压的重试队列，如果为真则清空重试队列，否则不清理。</param>
        void Stop(bool clean);

        /// <summary>
        /// 将指定的调度处理加入到重试队列。
        /// </summary>
        /// <param name="handler">指定要重试的处理器。</param>
        /// <param name="context">指定要重试的处理上下文对象。</param>
        /// <param name="exception">指定导致要重试的异常。</param>
        /// <remarks>
        ///		<para>该方法会自动触发启动操作。</para>
        /// </remarks>
        void Retry(IHandler handler, IHandlerContext context, Exception exception = null);
    }
}

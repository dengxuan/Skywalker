using Skywalker.Scheduler.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Scheduler
{
    public class Retriever : IRetriever
    {
        public event EventHandler<HandledEventArgs> Failed;
        public event EventHandler<HandledEventArgs> Succeed;
        public event EventHandler<HandledEventArgs> Discarded;

        private readonly AutoResetEvent _semaphore;
        private CancellationTokenSource _cancellation;
        private readonly ConcurrentQueue<RetryingToken> _queue;

        public Retriever()
        {
            _semaphore = new AutoResetEvent(true);
            _queue = new ConcurrentQueue<RetryingToken>();
        }

        public void Run()
        {
            //如果取消标记源不为空（表示启动过）并且没有被取消过，则表示当前状态为正常运行中
            if(_cancellation != null && !_cancellation.IsCancellationRequested)
            {
                return;
            }

            try
            {
                //等待信号量
                _semaphore.WaitOne();

                if(_cancellation == null || _cancellation.IsCancellationRequested)
                {
                    //重新创建一个新的取消标记源
                    _cancellation = new CancellationTokenSource();

                    //启动一个长期任务来运行重试处理方法
                    Task.Factory.StartNew(OnRetry,
                                          _cancellation.Token,
                                          _cancellation.Token,
                                          TaskCreationOptions.LongRunning,
                                          TaskScheduler.Default);
                }
            }
            finally
            {
                //恢复信号量
                _semaphore.Set();
            }
        }

        public void Stop(bool clean)
        {
            //如果取消标记源为空（表示还未启动过）或者已经被取消了（即已经停止了），则返回即可
            if(_cancellation == null || _cancellation.IsCancellationRequested)
            {
                //根据需要清空重试队列
                if(clean)
                {
                    this.ClearQueue();
                }
                return;
            }

            try
            {
                //等待信号量
                _semaphore.WaitOne();

                //如果没有取消标记源不为空（表示已启动过）并且未被取消，则取消它（即中断重试任务）
                if(_cancellation != null && !_cancellation.IsCancellationRequested)
                {
                    _cancellation.Cancel();
                }

                //根据需要清空重试队列
                if(clean)
                {
                    this.ClearQueue();
                }
            }
            finally
            {
                //释放信号量
                _semaphore.Set();
            }
        }

        public void Retry(IHandler handler, IHandlerContext context, Exception exception)
        {
            //获取下次触发的时间点
            var expiration = context.Trigger.GetNextOccurrence();

            //将处理器加入到重试队列
            _queue.Enqueue(new RetryingToken(handler, context, this.GetExpiration(handler, context, expiration), exception));

            //如果取消源为空（未启动过重试任务）或已经被取消（即重试任务被中断），则应该重启重试任务
            if(_cancellation == null || _cancellation.IsCancellationRequested)
            {
                this.Run();
            }
        }

        private void OnRetry(object state)
        {
            var cancellation = (CancellationToken)state;

            while(!cancellation.IsCancellationRequested)
            {
                //如果重试队列空了，那就线程休眠1秒钟
                if(_queue.IsEmpty)
                {
                    Thread.Sleep(1000);
                }

                if(_queue.TryDequeue(out var token))
                {
                    //获取待重试的处理器的延迟执行时间
                    var latency = this.GetLatency(token);

                    //如果延迟执行时间为空则表示已经过期（即再也不用重试了，弃之）
                    if(latency == null)
                    {
                        //激发“Discarded”事件
                        this.OnDiscarded(token.Handler, token.Context, token.Exception);

                        continue;
                    }

                    //如果延迟时间大于当前时间，则应跳过当前项
                    if(latency.Value > DateTime.Now)
                    {
                        //如果重试队列空了，则表示该执行项很可能是唯一需要重试项
                        //那么需要暂停一秒（重试最小间隔单位），以避免频繁空处理
                        if(_queue.IsEmpty)
                        {
                            Thread.Sleep(1000);
                        }

                        //将该未到延迟执行的处理器重新入队
                        _queue.Enqueue(token);

                        //继续队列中下一个
                        continue;
                    }

                    //定义重试失败的标记
                    var isFailed = false;

                    try
                    {
                        //更新上下文中的重试信息
                        token.Context.Failure = new HandlerFailure(token.RetriedCount, token.RetriedTimestamp, token.Expiration);

                        //调用处理器的处理方法
                        token.Handler.Handle(token.Context);

                        //将最新失败异常置空
                        token.Exception = null;
                    }
                    catch(Exception ex)
                    {
                        //表示重试失败
                        isFailed = true;

                        //更新最新的失败异常
                        token.Exception = ex;

                        //将重试失败的句柄重新入队
                        _queue.Enqueue(token);
                    }

                    //递增重试次数
                    token.RetriedCount++;

                    //更新处理器的最后重试时间
                    token.RetriedTimestamp = DateTime.Now;

                    //更新上下文中的重试信息
                    token.Context.Failure = new HandlerFailure(token.RetriedCount, token.RetriedTimestamp, token.Expiration);

                    //激发重试失败或成功的事件
                    if(isFailed)
                    {
                        this.OnFailed(token.Handler, token.Context, token.Exception);
                    }
                    else
                    {
                        this.OnSucceed(token.Handler, token.Context);
                    }
                }
            }
        }

        protected virtual DateTime? GetExpiration(IHandler handler, IHandlerContext context, DateTime? timestamp)
        {
            //如果下次触发时间为空（即没有后续触发了），则返回空（不限制）
            if(timestamp == null)
            {
                return null;
            }

            //计算距离下次触发的间隔时长
            TimeSpan duration = Utility.GetDuration(timestamp.Value);

            //如果下次触发时间还小于当前，则返回下次触发时间为限制时
            if(duration <= TimeSpan.Zero)
            {
                return timestamp;
            }

            //如果大于1天，则按每天递减1小时，递减量最多不超过24小时
            if(duration.TotalDays > 1)
            {
                return timestamp.Value.AddHours(-Math.Min(duration.TotalDays, 24));
            }

            //如果大于1小时并小于等于24小时，则按每小时递减10分钟，递减量最多不超过240分钟（4个小时）
            if(duration.TotalHours > 1)
            {
                return timestamp.Value.AddMinutes(-(duration.TotalHours * 10));
            }

            //如果大于1分钟并小于等于60分钟，则按每分钟递减10秒钟，递减量最多不超过600秒（10分钟）
            if(duration.TotalMinutes > 1)
            {
                return timestamp.Value.AddSeconds(-(duration.TotalMinutes * 10));
            }

            //小于或等于1分钟，则每10秒钟递减1秒钟，递减量为1秒至6秒
            return timestamp.Value.AddSeconds(-Math.Max(duration.TotalSeconds / 10, 1));
        }

        protected virtual void OnFailed(IHandler handler, IHandlerContext context, Exception exception)
        {
            this.Failed?.Invoke(this, new HandledEventArgs(handler, context, exception));
        }

        protected virtual void OnSucceed(IHandler handler, IHandlerContext context)
        {
            this.Succeed?.Invoke(this, new HandledEventArgs(handler, context, null));
        }

        protected virtual void OnDiscarded(IHandler handler, IHandlerContext context, Exception exception)
        {
            this.Discarded?.Invoke(this, new HandledEventArgs(handler, context, exception));
        }

        private void ClearQueue()
        {
            while(!_queue.IsEmpty)
            {
                _queue.TryDequeue(out var _);
            }
        }

        private DateTime? GetLatency(RetryingToken token)
        {
            //如果待重试的处理器重试期限已过，则返回空（即忽略它）
            if(token.Expiration.HasValue && token.Expiration.Value < DateTime.Now)
            {
                return null;
            }

            //如果重试次数为零或最后重试时间为空则返回当前时间（即不需要延迟）
            if(token.RetriedCount < 1 || token.RetriedTimestamp == null)
            {
                return DateTime.Now;
            }

            var seconds = Math.Min(token.RetriedCount * 2, 60);
            var latency = token.RetriedTimestamp.Value.AddSeconds(seconds);

            //如果待重试项有最后期限时间并且计算后的延迟执行时间大于该期限值，则返回期限时
            if(token.Expiration.HasValue && latency > token.Expiration.Value)
            {
                return token.Expiration.Value;
            }

            return latency;
        }

        private class RetryingToken
        {
            public IHandler Handler;
            public IHandlerContext Context;
            public DateTime? Expiration;
            public DateTime? RetriedTimestamp;
            public int RetriedCount;
            public Exception Exception;

            public RetryingToken(IHandler handler, IHandlerContext context, DateTime? expiration, Exception exception)
            {
                this.Handler = handler;
                this.Context = context;
                this.Expiration = expiration;
                this.Exception = exception;
            }
        }
    }
}

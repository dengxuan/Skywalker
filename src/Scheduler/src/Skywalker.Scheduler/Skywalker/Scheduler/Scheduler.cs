using Skywalker.Scheduler.Abstractions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Scheduler
{
    public class Scheduler : Worker, IScheduler
    {

        private TaskToken? _token;
        private readonly ConcurrentDictionary<ITrigger, ScheduleToken> _schedules;
        private readonly HashSet<IHandler> _handlers;
        private readonly TriggerCollection _triggers;
        private IDictionary<string, object>? _states;
        private IRetriever _retriever;

        public event EventHandler<HandledEventArgs>? Handled;
        public event EventHandler<OccurredEventArgs>? Occurred;
        public event EventHandler<OccurringEventArgs>? Occurring;
        public event EventHandler<ScheduledEventArgs>? Scheduled;

        public Scheduler()
        {
            CanPauseAndContinue = true;

            _handlers = new HashSet<IHandler>();
            _schedules = new ConcurrentDictionary<ITrigger, ScheduleToken>(TriggerComparer.Instance);
            _triggers = new TriggerCollection(_schedules);
            _retriever = new Retriever();
        }

        public DateTime? NextTime
        {
            get
            {
                return _token?.Timestamp;
            }
        }

        public DateTime? LastTime { get; private set; }

        public IRetriever Retriever
        {
            get
            {
                return _retriever;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Retriever");
                }

                //如果新值与原有值引用相等则忽略
                if (ReferenceEquals(_retriever, value))
                {
                    return;
                }

                //更新属性值
                var original = Interlocked.Exchange(ref _retriever, value);

                //通知子类该属性值发生了改变
                OnRetrieverChanged(value, original);
            }
        }

        public IReadOnlyCollection<ITrigger> Triggers
        {
            get
            {
                return _triggers;
            }
        }

        public IReadOnlyCollection<IHandler> Handlers
        {
            get
            {
                return _handlers;
            }
        }

        public bool IsScheduling
        {
            get
            {
                var token = _token;
                return token != null && !token.IsCancellationRequested;
            }
        }

        public bool HasStates
        {
            get
            {
                return _states != null && _states.Count > 0;
            }
        }

        public IDictionary<string, object> States
        {
            get
            {
                if (_states == null)
                {
                    Interlocked.CompareExchange(ref _states, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);
                }

                return _states;
            }
        }

        public IEnumerable<IHandler> GetHandlers(ITrigger trigger)
        {
            if (trigger == null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            if (_schedules.TryGetValue(trigger, out var schedule))
            {
                return schedule.Handlers;
            }
            else
            {
                return Enumerable.Empty<IHandler>();
            }
        }

        public bool Schedule(IHandler handler, ITrigger trigger)
        {
            return Schedule(handler, trigger, null);
        }

        public bool Schedule(IHandler handler, ITrigger trigger, Action<IHandlerContext>? onTrigger)
        {
            if (trigger == null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (onTrigger != null)
            {
                //TODO: 暂时不支持该功能
                throw new NotSupportedException();
            }

            //将处理器增加到处理器集中，如果添加成功（说明该处理器没有被调度过）
            if (_handlers.Add(handler))
            {
                //将该处理器加入到指定的触发器中的调度处理集
                return ScheduleCore(handler, trigger);
            }

            return false;
        }

        public bool Reschedule(IHandler handler, ITrigger trigger)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (trigger == null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            //将处理器增加到处理器集中，如果添加成功（说明该处理器没有被调度过）
            if (_handlers.Add(handler))
            {
                //将该处理器加入到指定的触发器中的调度处理集
                return ScheduleCore(handler, trigger);
            }

            //定义找到的调度项变量（默认没有找到）
            ScheduleToken? found = null;

            //循环遍历排程集，查找重新排程的触发器
            foreach (var schedule in _schedules.Values)
            {
                //如果当前排程的触发器等于要重新排程的触发器，则更新找到引用
                if (schedule.Trigger.Equals(trigger))
                {
                    found = schedule;
                }
                else //否则就尝试将待排程的处理器从原有排程项的处理集中移除掉
                {
                    schedule.RemoveHandler(handler);
                }
            }

            if (found.HasValue)
            {
                //将指定的执行处理器加入到找到的调度项的执行集合中，如果加入成功则尝试重新激发
                //该新增方法确保同步完成，不会引发线程重入导致的状态不一致
                if (found.Value.AddHandler(handler))
                {
                    //尝试重新触发
                    Refire(found.Value);

                    //返回重新调度成功
                    return true;
                }

                //返回重新调度失败
                return false;
            }

            //将该处理器加入到指定的触发器中的调度处理集
            return ScheduleCore(handler, trigger);
        }

        public void Unschedule()
        {
            //将待触发的任务标记置空
            var token = Interlocked.Exchange(ref _token, null);

            //如果待触发的任务标记不为空，则将其取消
            if (token != null)
            {
                token.Cancel();
            }

            _handlers.Clear();
            _schedules.Clear();
        }

        public bool Unschedule(IHandler handler)
        {
            if (handler == null)
            {
                return false;
            }

            if (_handlers.Remove(handler))
            {
                foreach (var schedule in _schedules.Values)
                {
                    schedule.RemoveHandler(handler);
                    if (schedule.Count == 0)
                    {
                        _schedules.TryRemove(schedule.Trigger, out _);
                    }
                }

                return true;
            }

            return false;
        }

        public bool Unschedule(ITrigger trigger)
        {
            if (trigger == null)
            {
                return false;
            }

            if (_schedules.TryRemove(trigger, out var schedule))
            {
                schedule.ClearHandlers();
                return true;
            }

            return false;
        }

        protected override void OnStart(string[] args)
        {
            //扫描调度集
            Scan();

            //启动失败重试队列
            _retriever.Run();
        }

        protected override void OnStop(string[] args)
        {
            //将待触发的任务标记置空
            var token = Interlocked.Exchange(ref _token, null);

            //如果待触发的任务标记不为空，则将其取消
            if (token != null)
            {
                token.Cancel();
            }

            //清空处理器集
            _handlers.Clear();

            //清空调度项集
            _schedules.Clear();

            //停止失败重试队列并清空所有待重试项
            _retriever.Stop(true);
        }

        protected override void OnPause()
        {
            //将待触发的任务标记置空
            var token = Interlocked.Exchange(ref _token, null);

            //如果待触发的任务标记不为空，则将其取消
            if (token != null)
            {
                token.Cancel();
            }

            //停止失败重试队列
            _retriever.Stop(false);
        }

        protected override void OnResume()
        {
            //扫描调度集
            Scan();

            //启动失败重试队列
            _retriever.Run();
        }

        protected virtual void OnRetrieverChanged(IRetriever newRetriever, IRetriever oldRetriever)
        {
        }

        /// <summary>
        /// 重新扫描排程集，并规划最新的调度任务。
        /// </summary>
        /// <remarks>
        ///		<para>对调用者的建议：该方法只应在异步启动中调用。</para>
        /// </remarks>
        protected void Scan()
        {
            //如果排程集为空则退出扫描
            if (_schedules.IsEmpty)
            {
                return;
            }

            DateTime? earliest = null;
            var schedules = new List<ScheduleToken>();

            //循环遍历排程集，找出其中最早的触发时间点
            foreach (var schedule in _schedules.Values)
            {
                //如果当前排程项的处理器集空了，则忽略它
                if (schedule.Count == 0)
                {
                    continue;
                }

                //获取当前排程项的下次触发时间
                var timestamp = schedule.Trigger.GetNextOccurrence();

                if (timestamp.HasValue && (earliest == null || timestamp.Value <= earliest))
                {
                    //如果下次触发时间比之前找到的最早项还早，则将之前的排程列表清空
                    if (timestamp.Value < earliest)
                    {
                        schedules.Clear();
                    }

                    //更新当前最早触发时间点
                    earliest = timestamp.Value;

                    //将找到的最早排程项加入到列表中
                    schedules.Add(schedule);
                }
            }

            //如果找到最早的触发时间，则将找到的排程项列表加入到调度进程中
            if (earliest.HasValue)
            {
                Fire(earliest.Value, schedules);
            }
        }

        protected virtual void OnHandled(IHandler handler, IHandlerContext context, Exception exception)
        {
            Handled?.Invoke(this, new HandledEventArgs(handler, context, exception));
        }

        protected virtual void OnOccurred(string scheduleId, int count)
        {
            Occurred?.Invoke(this, new OccurredEventArgs(scheduleId, count));
        }

        protected virtual void OnOccurring(string scheduleId)
        {
            Occurring?.Invoke(this, new OccurringEventArgs(scheduleId));
        }

        protected virtual void OnScheduled(string scheduleId, int count, ITrigger[] triggers)
        {
            Scheduled?.Invoke(this, new ScheduledEventArgs(scheduleId, count, triggers));
        }

        private void Refire(ScheduleToken schedule)
        {
            //获取当前的任务标记
            var token = _token;

            //如果当前任务标记为空（表示还没有启动排程）或任务标记已经被取消过（表示任务处于暂停或停止状态）
            if (token == null || token.IsCancellationRequested)
            {
                return;
            }

            //获取下次触发的时间点
            var timestamp = schedule.Trigger.GetNextOccurrence();

            //如果下次触发时间不为空（即需要触发）
            if (timestamp.HasValue)
            {
                if (timestamp < token.Timestamp)
                {
                    //如果新得到的触发时间小于待触发的时间，则尝试调度新的时间点
                    Fire(timestamp.Value, new[] { schedule });
                }

                else if (timestamp == token.Timestamp)
                {
                    //如果新得到的触发时间等于待触发的时间，则尝试将其加入到待触发任务中
                    token.Append(schedule, (id, count, triggers) =>
                    {
                        //激发“Scheduled”事件
                        if(!triggers.IsNullOrEmpty())
                        {
                            OnScheduled(id, count, triggers!);
                        }
                    });
                }
            }
        }

        private void Fire(DateTime timestamp, IEnumerable<ScheduleToken> schedules)
        {
            if (schedules == null)
            {
                return;
            }

            //首先获取待处理的任务凭证
            var pendding = _token;

            //如果待处理的任务凭证有效并且指定要重新触发的时间大于或等于待触发时间则忽略当次调度
            if (pendding != null && timestamp >= pendding.Timestamp)
            {
                return;
            }

            //创建一个新的任务凭证
            var current = new TaskToken(timestamp, schedules);

            //循环确保本次替换的任务凭证没有覆盖到其他线程乱入的
            while (pendding == null || timestamp < pendding.Timestamp)
            {
                //将新的触发凭证设置到全局变量，并确保该设置不会覆盖其他线程的乱入
                var last = Interlocked.CompareExchange(ref _token, current, pendding);

                //如果设置成功则退出该循环
                if (last == pendding)
                {
                    break;
                }
                else
                {
                    //设置失败：表示中间有其他线程的乱入，则将乱入的最新值设置为比对的凭证
                    pendding = last;
                }
            }

            //注意：再次确认待处理的任务凭证有效并且指定要重新触发的时间大于或等于待触发时间则忽略当次调度
            if (pendding != null && timestamp >= pendding.Timestamp)
            {
                //将刚创建的任务标记销毁
                current.Dispose();

                //退出
                return;
            }

            //如果原有任务凭证不为空，则将原有任务取消掉
            if (pendding != null)
            {
                pendding.Cancel();
            }

            //获取延迟的时长
            var duration = Utility.GetDuration(timestamp);

            Task.Delay(duration).ContinueWith((task, state) =>
            {
                //获取当前的任务调度凭证
                if (state is not TaskToken token)
                {
                    throw new ArgumentNullException(nameof(state));
                }
                //注意：防坑处理！！！
                //任务线程可能没有延迟足够的时长就提前进入，所以必须防止这种提前进入导致的触发器的触发时间计算错误
                if (Utility.Now() < token.Timestamp)
                {
                    SpinWait.SpinUntil(() => token.IsCancellationRequested || DateTime.Now.Ticks >= token.Timestamp.Ticks);
                }

                //如果任务已经被取消，则退出
                if (token.IsCancellationRequested)
                {
                    return;
                }

                //将最近触发时间点设为此时此刻
                LastTime = token.Timestamp;

                //注意：必须将待处理任务标记置空（否则会误导Scan方法重新进入Fire方法内的有效性判断）
                _token = null;

                //启动新一轮的调度扫描
                Scan();

                //设置处理次数
                int count = 0;

                //激发“Occurring”事件
                OnOccurring(token.Identity);

                //遍历待执行的调度项集合（该集合内部确保了线程安全）
                foreach (var schedule in token.Schedules)
                {
                    //遍历当前调度项内的所有处理器集合（该集合内部确保了线程安全）
                    foreach (var handler in schedule.Handlers)
                    {
                        //创建处理上下文对象
                        var context = new HandlerContext(this, schedule.Trigger, token.Identity, token.Timestamp, count++);

                        Task.Run(() =>
                        {
                            //异步调用处理器进行处理（该方法内会屏蔽异常，并对执行异常的处理器进行重发处理）
                            return Handle(handler, context);
                        })
                        .ContinueWith(t =>
                        {
                            //异步调用处理器完成后，再激发“Handled”事件
                            OnHandled(handler, context, t.Result);
                        });
                    }
                }

                //激发“Occurred”事件
                OnOccurred(token.Identity, count);
            }, current, current.GetToken());

            try
            {
                //激发“Scheduled”事件
                OnScheduled(current.Identity, schedules.Sum(p => p.Count), schedules.Select(p => p.Trigger).ToArray());
            }
            catch (Exception ex)
            {
                //Todo Logging

            }
        }

        private Exception? Handle(IHandler handler, IHandlerContext context)
        {
            try
            {
                //调用处理器进行处理
                handler.Handle(context);

                //返回调用成功
                return null;
            }
            catch (Exception ex)
            {
                //将失败的处理器加入到重试队列中
                _retriever.Retry(handler, context, ex);

                //返回调用失败
                return ex;
            }
        }

        private bool ScheduleCore(IHandler handler, ITrigger trigger)
        {
            //获取指定触发器关联的执行处理器集合
            var schedule = _schedules.GetOrAdd(trigger, key => new ScheduleToken(key, new HashSet<IHandler>()));

            //将指定的执行处理器加入到对应的调度项的执行集合中，如果加入成功则尝试重新激发
            //该新增方法确保同步完成，不会引发线程重入导致的状态不一致
            if (schedule.AddHandler(handler))
            {
                //尝试重新调度
                Refire(schedule);

                //返回新增调度成功
                return true;
            }

            //返回默认失败
            return false;
        }

        private class TaskToken : IDisposable
        {
            public readonly string Identity;
            public readonly DateTime Timestamp;

            private CancellationTokenSource _cancellation;
            private readonly ISet<ScheduleToken> _schedules;

            public TaskToken(DateTime timestamp, IEnumerable<ScheduleToken> schedules)
            {
                Timestamp = timestamp;
                Identity = Randomizer.GenerateString();
                _schedules = new HashSet<ScheduleToken>(schedules);
                _cancellation = new CancellationTokenSource();
            }

            public bool IsCancellationRequested
            {
                get
                {
                    var cancellation = _cancellation;
                    return cancellation == null || cancellation.IsCancellationRequested;
                }
            }

            public IEnumerable<ScheduleToken> Schedules
            {
                get
                {
                    var schedules = _schedules;

                    if (schedules == null)
                    {
                        yield break;
                    }

                    lock (schedules)
                    {
                        foreach (var schedule in schedules)
                        {
                            yield return schedule;
                        }
                    }
                }
            }

            public bool Append(ScheduleToken token, Action<string, int, ITrigger[]?> succeed)
            {
                var schedules = _schedules;

                if (schedules == null)
                {
                    return false;
                }

                var result = false;
                var count = 0;
                ITrigger[]? triggers = null;

                lock (schedules)
                {
                    //将指定的调度项加入
                    result = schedules.Add(token);

                    //如果追加成功，则必须在同步临界区内进行统计
                    if (result && succeed != null)
                    {
                        //计算调度任务中的处理器总数
                        count = schedules.Sum(p => p.Count);

                        //获取调度任务中的触发器集合
                        triggers = schedules.Select(p => p.Trigger).ToArray();
                    }
                }

                //如果增加成功并且回调方法不为空，则回调成功方法
                if (result && succeed != null)
                {
                    succeed(Identity, count, triggers);
                }

                return result;
            }

            public CancellationToken GetToken()
            {
                return _cancellation.Token;
            }

            public void Cancel()
            {
                var cancellation = _cancellation;

                if (cancellation != null)
                {
                    cancellation.Cancel();
                }
            }

            public void Dispose()
            {
                var cancellation = Interlocked.Exchange(ref _cancellation, null);

                if (cancellation != null)
                {
                    _cancellation.Cancel();
                    _cancellation.Dispose();
                }
            }
        }

        private struct ScheduleToken : IEquatable<ScheduleToken>, IEquatable<ITrigger>
        {
            public readonly ITrigger Trigger;

            private readonly ISet<IHandler> _handlers;
            private readonly AutoResetEvent _semaphore;

            public ScheduleToken(ITrigger trigger, ISet<IHandler> handlers)
            {
                Trigger = trigger;
                _handlers = handlers;
                _semaphore = new AutoResetEvent(true);
            }

            /// <summary>
            /// 获取调度项包含的处理器数量。
            /// </summary>
            public int Count
            {
                get
                {
                    return _handlers.Count;
                }
            }

            /// <summary>
            /// 获取调度项包含的处理器集，该集内部以独占方式提供遍历。
            /// </summary>
            public IEnumerable<IHandler> Handlers
            {
                get
                {
                    try
                    {
                        _semaphore.WaitOne();

                        foreach (var handler in _handlers)
                        {
                            yield return handler;
                        }
                    }
                    finally
                    {
                        _semaphore.Set();
                    }
                }
            }

            /// <summary>
            /// 以同步方式将指定的处理器加入到当前调度项中。
            /// </summary>
            /// <param name="handler">指定要添加的同步器。</param>
            /// <returns>添加成功返回真，否则返回假（即表示指定的同步器已经存在于当前调度项中了）。</returns>
            public bool AddHandler(IHandler handler)
            {
                if (handler == null)
                {
                    return false;
                }

                try
                {
                    _semaphore.WaitOne();

                    return _handlers.Add(handler);
                }
                finally
                {
                    _semaphore.Set();
                }
            }

            /// <summary>
            /// 以同步方式将指定的处理器从当前调度项中移除。
            /// </summary>
            /// <param name="handler">指定要移除的同步器。</param>
            /// <returns>移除成功返回真，否则返回假（即表示指定的同步器已经不存在于当前调度项中了）。</returns>
            public bool RemoveHandler(IHandler handler)
            {
                if (handler == null)
                {
                    return false;
                }

                try
                {
                    _semaphore.WaitOne();

                    return _handlers.Remove(handler);
                }
                finally
                {
                    _semaphore.Set();
                }
            }

            /// <summary>
            /// 以同步方式将当前调度项中的处理器集清空。
            /// </summary>
            public void ClearHandlers()
            {
                try
                {
                    _semaphore.WaitOne();

                    _handlers.Clear();
                }
                finally
                {
                    _semaphore.Set();
                }
            }

            public bool Equals(ITrigger? trigger)
            {
                return Trigger.Equals(trigger);
            }

            public bool Equals(ScheduleToken other)
            {
                return Trigger.Equals(other.Trigger);
            }

            public override bool Equals(object? @object)
            {
                if (@object == null || @object.GetType() != GetType())
                {
                    return false;
                }

                return base.Equals((ScheduleToken)@object);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Trigger);
            }

            public override string ToString()
            {
                return Trigger.ToString() + " (" + _handlers.Count + ")";
            }
        }

        private class TriggerComparer : IEqualityComparer<ITrigger>
        {
            public static readonly TriggerComparer Instance = new TriggerComparer();

            private TriggerComparer() { }

            public bool Equals(ITrigger? x, ITrigger? y)
            {
                if (x == null || y == null)
                {
                    return false;
                }

                return x.GetType() == y.GetType() && x.Equals(y);
            }

            public int GetHashCode(ITrigger obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                return obj.GetType().GetHashCode() ^ obj.GetHashCode();
            }
        }

        private class TriggerCollection : IReadOnlyCollection<ITrigger>
        {
            private readonly IDictionary<ITrigger, ScheduleToken> _schedules;

            public TriggerCollection(IDictionary<ITrigger, ScheduleToken> schedules)
            {
                _schedules = schedules ?? throw new ArgumentNullException(nameof(schedules));
            }

            public int Count
            {
                get
                {
                    return _schedules.Count;
                }
            }

            public IEnumerator<ITrigger> GetEnumerator()
            {
                foreach (var schedule in _schedules)
                {
                    yield return schedule.Key;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}

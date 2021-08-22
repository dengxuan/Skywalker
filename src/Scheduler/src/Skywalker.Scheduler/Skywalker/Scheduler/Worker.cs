using Skywalker.Scheduler.Abstractions;
using System;
using System.Threading;

namespace Skywalker.Scheduler
{
    /// <summary>
    /// 这是工作器的基类。
    /// </summary>
    /// <remarks>
    ///		<para>该实现提供了对<see cref="OnStart(string[])"/>、<see cref="OnStop(string[])"/>、<see cref="OnPause"/>、<see cref="OnResume"/>这四个方法之间的线程重入的隔离。</para>
    ///		<para>对于子类的实现者而言，无需担心这些方法会在多线程中会导致状态的不一致，并确保了它们不会发生线程重入。</para>
    /// </remarks>
    public abstract class Worker : IWorker, IDisposable
    {
        private const int DISPOSED_FLAG = 1;

        public event EventHandler<WorkerStateChangedEventArgs> StateChanged;

        private string _name;
        private bool _enabled;
        private bool _canPauseAndContinue;
        private int _state;
        private int _disposing;
        private readonly AutoResetEvent _semaphore;

        protected Worker() : this(null) { }

        protected Worker(string name)
        {
            _name = string.IsNullOrWhiteSpace(name) ? this.GetType().Name : name.Trim();
            _enabled = true;
            _canPauseAndContinue = false;
            _state = (int)WorkerState.Stopped;
            _semaphore = new AutoResetEvent(true);
        }

        /// <summary>
        /// 获取或设置工作器的名称。
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            protected set
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }

                _name = value;
            }
        }

        /// <summary>
        /// 获取或设置是否禁用当前工作器。
        /// </summary>
        /// <remarks>如果当前工作器</remarks>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if(_enabled == value)
                {
                    return;
                }

                _enabled = value;

                if(!value)
                {
                    this.Stop();
                }
            }
        }

        /// <summary>
        /// 获取或设置工作器工作器是否可以暂停和继续。
        /// </summary>
        public bool CanPauseAndContinue
        {
            get
            {
                return _canPauseAndContinue;
            }
            protected set
            {
                if(_state != (int)WorkerState.Stopped)
                {
                    throw new InvalidOperationException();
                }

                _canPauseAndContinue = value;
            }
        }

        /// <summary>
        /// 获取工作器的状态。
        /// </summary>
        public WorkerState State { get { return (WorkerState)_state; } }

        public void Start(params string[] args)
        {
            //如果不可用或当前状态不是已停止则返回
            if(!_enabled || _state != (int)WorkerState.Stopped)
            {
                return;
            }

            try
            {
                //等待信号量
                _semaphore.WaitOne();

                if(!_enabled || _state != (int)WorkerState.Stopped)
                {
                    return;
                }

                //更新当前状态为“正在启动中”
                _state = (int)WorkerState.Starting;

                //调用启动抽象方法，以执行实际的启动操作
                this.OnStart(args);

                //更新当前状态为“运行中”
                _state = (int)WorkerState.Running;

                //激发“StateChanged”事件
                this.OnStateChanged(nameof(Start), WorkerState.Running);
            }
            catch(Exception ex)
            {
                _state = (int)WorkerState.Stopped;

                //激发“StateChanged”事件
                this.OnStateChanged(nameof(Start), WorkerState.Stopped, ex);

                throw;
            }
            finally
            {
                //释放信号量
                _semaphore.Set();
            }
        }

        public void Stop(params string[] args)
        {
            if(_state == (int)WorkerState.Stopping || _state == (int)WorkerState.Stopped)
            {
                return;
            }

            int originalState = -1;

            try
            {
                //等待信号量
                _semaphore.WaitOne();

                if(_state == (int)WorkerState.Stopping || _state == (int)WorkerState.Stopped)
                {
                    return;
                }

                //更新当前状态为“正在停止中”，并保存原来的状态
                originalState = Interlocked.Exchange(ref _state, (int)WorkerState.Stopping);

                //调用停止抽象方法，以执行实际的停止操作
                this.OnStop(args);

                //更新当前状态为已停止
                _state = (int)WorkerState.Stopped;

                //激发“StateChanged”事件
                this.OnStateChanged(nameof(Stop), WorkerState.Stopped);
            }
            catch(Exception ex)
            {
                //还原状态
                _state = originalState;

                //激发“StateChanged”事件
                this.OnStateChanged(nameof(Stop), (WorkerState)originalState, ex);

                throw;
            }
            finally
            {
                //释放信号量
                _semaphore.Set();
            }
        }

        public void Pause()
        {
            //如果不可用则退出
            if(!_enabled)
            {
                return;
            }

            //如果不支持暂停继续则抛出异常
            if(!_canPauseAndContinue)
            {
                throw new NotSupportedException($"The {_name} worker does not support the Pause/Resume operation.");
            }

            if(_state != (int)WorkerState.Running)
            {
                return;
            }

            int originalState = -1;

            try
            {
                //等待信号量
                _semaphore.WaitOne();

                if(_state != (int)WorkerState.Running)
                {
                    return;
                }

                //更新当前状态为“正在暂停中”，并保存原来的状态
                originalState = Interlocked.Exchange(ref _state, (int)WorkerState.Pausing);

                //执行暂停操作
                this.OnPause();

                //更新当前状态为“已经暂停”
                _state = (int)WorkerState.Paused;

                //激发“StateChanged”事件
                this.OnStateChanged(nameof(Pause), WorkerState.Paused);
            }
            catch(Exception ex)
            {
                //还原状态
                _state = originalState;

                //激发“StateChanged”事件
                this.OnStateChanged(nameof(Pause), (WorkerState)originalState, ex);

                throw;
            }
            finally
            {
                //释放信号量
                _semaphore.Set();
            }
        }

        public void Resume()
        {
            //如果不可用则退出
            if(!_enabled)
            {
                return;
            }

            //如果不支持暂停继续则抛出异常
            if(!_canPauseAndContinue)
            {
                throw new NotSupportedException($"The {_name} worker does not support the Pause/Resume operation.");
            }

            if(_state != (int)WorkerState.Paused)
            {
                return;
            }

            int originalState = -1;

            try
            {
                //等待信号量
                _semaphore.WaitOne();

                if(_state != (int)WorkerState.Paused)
                {
                    return;
                }

                //更新当前状态为“正在恢复中”，并保存原来的状态
                originalState = Interlocked.Exchange(ref _state, (int)WorkerState.Resuming);

                //执行恢复操作
                this.OnResume();

                _state = (int)WorkerState.Running;

                //激发“StateChanged”事件
                this.OnStateChanged(nameof(Resume), WorkerState.Running);
            }
            catch(Exception ex)
            {
                //还原状态
                _state = originalState;

                //激发“StateChanged”事件
                this.OnStateChanged(nameof(Resume), (WorkerState)originalState, ex);

                throw;
            }
            finally
            {
                //释放信号量
                _semaphore.Set();
            }
        }

        protected abstract void OnStart(string[] args);

        protected abstract void OnStop(string[] args);

        protected virtual void OnPause() { }

        protected virtual void OnResume() { }

        public override string ToString()
        {
            if(_enabled)
            {
                return $"[{_state}] {_name}";
            }
            else
            {
                return $"[{_state}](Disabled) {_name}";
            }
        }

        protected virtual void OnStateChanged(string actionName, WorkerState state, Exception? exception = null)
        {
            this.StateChanged?.Invoke(this, new WorkerStateChangedEventArgs(actionName, state, exception));
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Stop();
        }

        void IDisposable.Dispose()
        {
            var original = Interlocked.Exchange(ref _disposing, DISPOSED_FLAG);

            if(original != DISPOSED_FLAG)
            {
                this.Dispose(true);
                _semaphore.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}

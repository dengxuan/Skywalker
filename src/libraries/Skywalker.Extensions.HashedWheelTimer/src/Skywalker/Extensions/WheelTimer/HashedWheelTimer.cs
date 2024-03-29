﻿using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Skywalker.Extensions.WheelTimer.Abstractions;

namespace Skywalker.Extensions.WheelTimer;

public class HashedWheelTimer : IWheelTimer
{
    private readonly ILogger<HashedWheelTimer> _logger;
    private static readonly AtomicInteger s_instanceCounter = new();
    private static readonly int s_instanceCountLimit = 64;
    private readonly long _tickDuration;
    private readonly HashedWheelBucket[] _wheel;
    private readonly int _mask;
    private readonly ManualResetEvent _startTimeInitialized = new(false);
    private ManualResetEvent? _workStopped;
    private readonly ConcurrentQueue<HashedWheelTimeout> _timeouts = new();

    private readonly ConcurrentQueue<HashedWheelTimeout> _cancelledTimeouts = new();

    private readonly ISet<IWheelTimeout> _unprocessedTimeouts = new HashSet<IWheelTimeout>();
    private readonly AtomicLong _pendingTimeouts = new();
    private readonly long _maxPendingTimeouts;
    private int _workerStarted;
    private CancellationTokenSource? _cancellationTokenSource;
    private long _tick;

    public long StartTime { get; private set; }

    public long PendingTimeouts => _pendingTimeouts.Value;

    internal void EnqueueCanceledTimeout(HashedWheelTimeout timeout)
    {
        _cancelledTimeouts.Enqueue(timeout);
    }

    internal void DecrementPendingTimeouts()
    {
        _pendingTimeouts.DecrementAndGet();
    }

    public HashedWheelTimer() : this(null, TimeSpan.FromMilliseconds(100))
    {
    }

    public HashedWheelTimer(TimeSpan span, int ticksPerWheel = 512, long maxPendingTimeouts = 0) : this(null, span, ticksPerWheel, maxPendingTimeouts)
    {
    }

    public HashedWheelTimer(ILogger<HashedWheelTimer>? logger, TimeSpan span, int ticksPerWheel = 512, long maxPendingTimeouts = 0)
    {
        if (ticksPerWheel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ticksPerWheel), "must be greater than 0: ");
        }

        _logger = logger ?? NullLogger<HashedWheelTimer>.Instance;

        // Normalize ticksPerWheel to power of two and initialize the wheel.
        _wheel = CreateWheel(ticksPerWheel);
        _mask = _wheel.Length - 1;

        var tickDuration = span.Milliseconds;
        if (tickDuration >= long.MaxValue / _wheel.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(tickDuration), "{tickDuration} (expected: 0 < tickDuration in ms < {long.MaxValue / _wheel.Length}");
        }

        if (tickDuration < 1)
        {
            _logger.LogWarning("Configured tickDuration {tickDuration} smaller then  1, using 1ms.", tickDuration);
            _tickDuration = 1;
        }
        else
        {
            _tickDuration = tickDuration;
        }

        _maxPendingTimeouts = maxPendingTimeouts;

        if (s_instanceCounter.IncrementAndGet() > s_instanceCountLimit)
        {
            var name = nameof(HashedWheelTimer);
            _logger.LogError("You are creating too many {name} instances. {name} is a shared resource that must be reused across the JVM, so that only a few instances are created.", name);
        }
    }

    private static HashedWheelBucket[] CreateWheel(int ticksPerWheel)
    {
        if (ticksPerWheel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ticksPerWheel), "must be greater than 0");
        }

        if (ticksPerWheel > 1073741824)
        {
            throw new ArgumentOutOfRangeException(nameof(ticksPerWheel), "may not be greater than 2^30");
        }

        ticksPerWheel = NormalizeTicksPerWheel(ticksPerWheel);
        var wheel = new HashedWheelBucket[ticksPerWheel];
        for (var i = 0; i < wheel.Length; i++)
        {
            wheel[i] = new HashedWheelBucket();
        }

        return wheel;
    }

    /// <summary>
    /// Schedules the specified TimerTask for one-time execution after the specified delay.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="span"></param>
    /// <returns>a handle which is associated with the specified task</returns>
    public IWheelTimeout NewTimeout(IWheelTimerTask task, TimeSpan span)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        if (_cancellationTokenSource != default && _cancellationTokenSource.Token.IsCancellationRequested)
        {
            throw new InvalidOperationException("cannot be started once stopped");
        }

        var pendingTimeoutsCount = _pendingTimeouts.IncrementAndGet();
        if (_maxPendingTimeouts > 0 && pendingTimeoutsCount > _maxPendingTimeouts)
        {
            _pendingTimeouts.DecrementAndGet();
            throw new InvalidOperationException(
                $"Number of pending timeouts ({pendingTimeoutsCount}) is greater than or equal to maximum allowed pending  timeouts ({_maxPendingTimeouts})");
        }

        Start();

        // Add the timeout to the timeout queue which will be processed on the next tick.
        // During processing all the queued HashedWheelTimeouts will be added to the correct HashedWheelBucket.
        var deadline = DateTimeHelper.TotalMilliseconds + (long)span.TotalMilliseconds - StartTime;

        // Guard against overflow.
        if (span.Milliseconds > 0 && deadline < 0)
        {
            deadline = long.MaxValue;
        }

        var timeout = new HashedWheelTimeout(this, task, deadline);
        _timeouts.Enqueue(timeout);
        return timeout;
    }

    public IEnumerable<IWheelTimeout> Stop()
    {
        if (_cancellationTokenSource == null || _cancellationTokenSource.Token.IsCancellationRequested)
        {
            return Enumerable.Empty<IWheelTimeout>();
        }

        try
        {
            _cancellationTokenSource.Cancel();
            _workStopped?.WaitOne();
        }
        finally
        {
            s_instanceCounter.DecrementAndGet();
        }

        return _unprocessedTimeouts;
    }

    public void Dispose()
    {
        s_instanceCounter.DecrementAndGet();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///  Starts the background thread explicitly.  The background thread will start automatically on demand 
    ///  even if you did not call this method.
    /// </summary>
    private void Start()
    {
        var workerStarted = Interlocked.CompareExchange(ref _workerStarted, 1, 0);
        if (workerStarted == 0)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _workStopped = new ManualResetEvent(false);
            Task.Factory.StartNew(async state =>
            {
                var cancellationToken = (CancellationToken)state!;
                StartTime = DateTimeHelper.TotalMilliseconds;
                if (StartTime == 0)
                {
                    // We use 0 as an indicator for the uninitialized value here, so make sure it's not 0 when initialized.
                    StartTime = 1;
                }

                // Notify the other threads waiting for the initialization at start().
                _startTimeInitialized.Set();

                do
                {
                    var deadline = await WaitForNextTickAsync();
                    if (deadline > 0)
                    {
                        var idx = (int)(_tick & _mask);
                        ProcessCancelledTasks();
                        var bucket = _wheel[idx];
                        TransferTimeoutsToBuckets();
                        bucket.ExpireTimeouts(deadline);
                        _tick++;
                    }
                } while (!cancellationToken.IsCancellationRequested);

                // Fill the unprocessedTimeouts so we can return them from stop() method.
                foreach (var bucket in _wheel)
                {
                    bucket.ClearTimeouts(_unprocessedTimeouts);
                }

                for (; ; )
                {
                    if (!_timeouts.TryDequeue(out var timeout) || timeout == null)
                    {
                        break;
                    }

                    if (!timeout.Cancelled)
                    {
                        _unprocessedTimeouts.Add(timeout);
                    }
                }

                ProcessCancelledTasks();

                // Notify the other threads work is stopped.
                _workStopped.Set();
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
        }

        // Wait until the startTime is initialized by the worker.
        while (StartTime == 0)
        {
            try
            {
                _startTimeInitialized.WaitOne(5000);
            }
            catch
            {
                // Ignore - it will be ready very soon.
            }
        }
    }

    private void TransferTimeoutsToBuckets()
    {
        // transfer only max. 100000 timeouts per tick to prevent a thread to stale the workerThread when it just
        // adds new timeouts in a loop.
        for (var i = 0; i < 100000; i++)
        {
            if (!_timeouts.TryDequeue(out var timeout) || timeout == null)
            {
                // all processed
                break;
            }

            if (timeout.State == TimeoutState.Cancelled)
            {
                // Was cancelled in the meantime.
                continue;
            }

            var calculated = timeout.Deadline / _tickDuration;
            timeout.RemainingRounds = (calculated - _tick) / _wheel.Length;

            var ticks = Math.Max(calculated, _tick); // Ensure we don't schedule for past.
            var stopIndex = (int)(ticks & _mask);

            var bucket = _wheel[stopIndex];
            bucket.AddTimeout(timeout);
        }
    }

    private void ProcessCancelledTasks()
    {
        while (true)
        {
            if (!_cancelledTimeouts.TryDequeue(out var timeout) || timeout == null)
            {
                // all processed
                break;
            }

            try
            {
                timeout.Remove();
            }
            catch (Exception e)
            {
                _logger.LogWarning("An exception was thrown while process a cancellation task", e);
            }
        }
    }

    private async Task<long> WaitForNextTickAsync()
    {
        var deadline = _tickDuration * (_tick + 1);

        while (true)
        {
            var currentTime = DateTimeHelper.TotalMilliseconds - StartTime;
            var sleepTimeMs = (int)Math.Truncate(deadline - currentTime + 1M);

            if (sleepTimeMs <= 0)
            {
                if (currentTime == long.MinValue)
                {
                    return -long.MaxValue;
                }
                else
                {
                    return currentTime;
                }
            }

            await Task.Delay(sleepTimeMs, default);
        }
    }

    private static int NormalizeTicksPerWheel(int ticksPerWheel)
    {
        var normalizedTicksPerWheel = 1;
        while (normalizedTicksPerWheel < ticksPerWheel)
        {
            normalizedTicksPerWheel <<= 1;
        }

        return normalizedTicksPerWheel;
    }
}

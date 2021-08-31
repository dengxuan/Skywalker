﻿using System;
using System.Collections.Generic;

namespace Skywalker.Extensions.RateLimiters;

/// <summary>
/// Used to control the rate of occurrences of an action per unit of time.
/// </summary>
/// <remarks>
/// The algorithm uses a rolling time window so that precise control over the rate is achieved (without any bursts)
/// </remarks>
public class RollingWindowThrottler
{
    private readonly object _locker = new();

    private readonly int _occurrences;

    private readonly long _timeUnitTicks;

    /// <summary>
    /// provides a queue of timestamps expressed in ticks when reservations expire
    /// </summary>
    private readonly Queue<long> _expirationTimestampsQueue;

    private long _nextCheckTime;

    private int _remainingTokens;

    /// <summary>
    /// Constructs an instance of the throttler.
    /// </summary>
    /// <param name="occurrences">Maximum number of occurences per time unit allowed.</param>
    /// <param name="timeUnit">The time unit in which the occurences are constrained.</param>
    public RollingWindowThrottler(int occurrences, TimeSpan timeUnit)
    {
        _occurrences = occurrences.Positive(nameof(occurrences));
        _timeUnitTicks = timeUnit.Ticks;
        _remainingTokens = occurrences;
        _expirationTimestampsQueue = new Queue<long>(occurrences);
    }


    /// <summary>
    /// Total number of occurrences of an action which are allowed for a time unit
    /// </summary>
    public int Occurrences
    {
        get { return _occurrences; }
    }

    /// <summary>
    /// The time unit in which the maximal number of <see cref="Occurrences"/> are allowed..
    /// </summary>
    public TimeSpan TimeUnit
    {
        get { return TimeSpan.FromTicks(_timeUnitTicks); }
    }

    /// <summary>
    /// Returns the number of available tokens which can be reserved until the end of current time unit
    /// </summary>
    public int AvailableTokens
    {
        get
        {
            lock (_locker)
            {
                return _remainingTokens;
            }
        }
    }

    /// <summary>
    /// Tries to reserve one token in the configured time unit.
    /// </summary>
    /// <param name="waitTimeMillis">total suggested wait time in milliseconds till tokens will become available for reservation</param>
    /// <returns>true if the caller should throttle/wait, or false if reservation was made successfully.</returns>
    public bool ShouldThrottle(out long waitTimeMillis)
    {
        return ShouldThrottle(1, out waitTimeMillis);
    }


    /// <summary>
    /// Tries to reserve <see cref="tokens"/> in the configured time unit.
    /// </summary>
    /// <param name="tokens">total number of reservations</param>
    /// <param name="waitTimeMillis">total suggested wait time in milliseconds till tokens will become available for reservation</param>
    /// <returns>true if the caller should throttle/wait, or false if reservation was made successfully.</returns>
    public bool ShouldThrottle(int tokens, out long waitTimeMillis)
    {
        tokens.Positive(nameof(tokens));
        var currentTime = DateTime.UtcNow.Ticks;

        lock (_locker)
        {
            CheckExitTimeQueue();
            if (_remainingTokens - tokens >= 0)
            {
                _remainingTokens -= tokens;
                waitTimeMillis = 0;
                long timeToExit = unchecked(currentTime + _timeUnitTicks);
                for (int i = 0; i < tokens; i++)
                    _expirationTimestampsQueue.Enqueue(timeToExit);
                return false;
            }

            waitTimeMillis = (_nextCheckTime - currentTime) / TimeSpan.TicksPerMillisecond;
            return true;
        }
    }

    private void CheckExitTimeQueue()
    {
        if (_nextCheckTime > DateTime.UtcNow.Ticks)
        {
            return;
        }

        while (_expirationTimestampsQueue.Count > 0 && _expirationTimestampsQueue.Peek() <= DateTime.UtcNow.Ticks)
        {
            _expirationTimestampsQueue.Dequeue();
            _remainingTokens++;
        }

        //try to determine next check time
        if (_expirationTimestampsQueue.Count > 0)
        {
            var item = _expirationTimestampsQueue.Peek();
            _nextCheckTime = item;
        }
        else
        {
            _nextCheckTime = _timeUnitTicks;
        }
    }

}

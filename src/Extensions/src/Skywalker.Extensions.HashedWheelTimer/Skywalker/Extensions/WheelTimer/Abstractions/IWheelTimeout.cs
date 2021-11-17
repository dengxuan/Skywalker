﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Extensions.WheelTimer.Abstractions
{
    /// <summary>
    /// A handle associated with a TimerTask that is returned by a Timer
    /// </summary>
    public interface IWheelTimeout
    {
        /// <summary>
        ///  Returns the Timer that created this handle.
        /// </summary>
        IWheelTimer Timer { get; }

        /// <summary>
        /// Returns the TimerTask which is associated with this handle.
        /// </summary>
        IWheelTimerTask TimerTask { get; }

        /// <summary>
        /// Returns true if and only if the TimerTask associated
        /// with this handle has been expired
        /// </summary>
        bool Expired { get; }

        /// <summary>
        /// Returns true if and only if the TimerTask associated
        /// with this handle has been cancelled
        /// </summary>
        bool Cancelled { get; }

        /// <summary>
        /// Attempts to cancel the {@link TimerTask} associated with this handle.
        /// If the task has been executed or cancelled already, it will return with no side effect.
        /// </summary>
        /// <returns>True if the cancellation completed successfully, otherwise false</returns>
        bool Cancel();
    }
}

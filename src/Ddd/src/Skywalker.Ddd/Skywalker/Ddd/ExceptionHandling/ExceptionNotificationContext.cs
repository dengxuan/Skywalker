using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.ExceptionHandling
{
    public class ExceptionNotificationContext
    {
        /// <summary>
        /// The exception object.
        /// </summary>
        [NotNull]
        public Exception Exception { get; }

        public LogLevel LogLevel { get; }

        /// <summary>
        /// True, if it is handled.
        /// </summary>
        public bool Handled { get; }

        public ExceptionNotificationContext(
            [NotNull] Exception exception,
            LogLevel? logLevel = null,
            bool handled = true)
        {
            Exception = exception.NotNull(nameof(exception));
            LogLevel = logLevel ?? LogLevel.Error;
            Handled = handled;
        }
    }
}
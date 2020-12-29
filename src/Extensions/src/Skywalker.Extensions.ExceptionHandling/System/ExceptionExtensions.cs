using Microsoft.Extensions.Logging;
using Skywalker.Logging;

namespace System
{
    public static class ExceptionExtensions
    {

        /// <summary>
        /// Try to get a log level from the given <paramref name="exception"/>
        /// if it implements the <see cref="IHasLogLevel"/> interface.
        /// Otherwise, returns the <paramref name="defaultLevel"/>.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="defaultLevel"></param>
        /// <returns></returns>
        public static LogLevel GetLogLevel(this Exception exception, LogLevel defaultLevel = LogLevel.Error)
        {
            return (exception as IHasLogLevel)?.LogLevel ?? defaultLevel;
        }
    }
}

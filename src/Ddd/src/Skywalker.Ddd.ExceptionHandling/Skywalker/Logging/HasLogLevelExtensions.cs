using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Logging
{
    public static class HasLogLevelExtensions
    {
        public static TException WithLogLevel<TException>([NotNull] this TException exception, LogLevel logLevel)
            where TException : class, IHasLogLevel
        {
            Check.NotNull(exception, nameof(exception));

            exception.LogLevel = logLevel;

            return exception;
        }
    }
}

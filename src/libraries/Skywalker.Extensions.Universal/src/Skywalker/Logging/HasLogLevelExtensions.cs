using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Skywalker.Logging;

public static class HasLogLevelExtensions
{
    public static TException WithLogLevel<TException>(this TException exception, LogLevel logLevel)
        where TException : class, IHasLogLevel
    {
        Check.NotNull(exception, nameof(exception));

        exception.LogLevel = logLevel;

        return exception;
    }
}

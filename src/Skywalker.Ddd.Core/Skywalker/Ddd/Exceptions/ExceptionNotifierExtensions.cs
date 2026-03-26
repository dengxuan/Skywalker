using Microsoft.Extensions.Logging;
using Skywalker.Ddd.Exceptions.Abstractions;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// 
/// </summary>
public static class ExceptionNotifierExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="exceptionNotifier"></param>
    /// <param name="exception"></param>
    /// <param name="logLevel"></param>
    /// <param name="handled"></param>
    /// <returns></returns>
    public static Task NotifyAsync(this IExceptionNotifier exceptionNotifier, Exception exception, LogLevel? logLevel = null, bool handled = true)
    {
        return exceptionNotifier.NotNull(nameof(exceptionNotifier)).NotifyAsync(new ExceptionNotificationContext(exception, logLevel, handled));
    }
}

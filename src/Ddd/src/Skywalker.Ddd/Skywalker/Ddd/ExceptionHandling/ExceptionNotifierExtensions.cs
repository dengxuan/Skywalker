using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.Ddd.ExceptionHandling
{
    public static class ExceptionNotifierExtensions
    {
        public static Task NotifyAsync([NotNull] this IExceptionNotifier exceptionNotifier, [NotNull] Exception exception, LogLevel? logLevel = null, bool handled = true)
        {
            return exceptionNotifier.NotNull(nameof(exceptionNotifier)).NotifyAsync(new ExceptionNotificationContext(exception, logLevel, handled));
        }
    }
}
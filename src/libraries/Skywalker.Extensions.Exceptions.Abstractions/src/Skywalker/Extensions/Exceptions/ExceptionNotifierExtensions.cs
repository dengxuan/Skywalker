﻿using Microsoft.Extensions.Logging;
using Skywalker.Extensions.Exceptions.Abstractions;

namespace Skywalker.Extensions.Exceptions;

public static class ExceptionNotifierExtensions
{
    public static Task NotifyAsync(this IExceptionNotifier exceptionNotifier, Exception exception, LogLevel? logLevel = null, bool handled = true)
    {
        return exceptionNotifier.NotNull(nameof(exceptionNotifier)).NotifyAsync(new ExceptionNotificationContext(exception, logLevel, handled));
    }
}

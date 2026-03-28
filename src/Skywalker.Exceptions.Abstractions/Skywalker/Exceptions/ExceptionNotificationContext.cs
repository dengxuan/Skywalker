using Microsoft.Extensions.Logging;

namespace Skywalker.Exceptions;

public class ExceptionNotificationContext
{
    public Exception Exception { get; }
    public LogLevel LogLevel { get; }
    public bool Handled { get; }

    public ExceptionNotificationContext(Exception exception, LogLevel? logLevel = null, bool handled = true)
    {
        Exception = exception.NotNull(nameof(exception));
        LogLevel = logLevel ?? LogLevel.Error;
        Handled = handled;
    }
}

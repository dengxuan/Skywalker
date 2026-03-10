using Microsoft.Extensions.Logging;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// 
/// </summary>
public class ExceptionNotificationContext
{
    /// <summary>
    /// The exception object.
    /// </summary>

    public Exception Exception { get; }

    /// <summary>
    /// 
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// True, if it is handled.
    /// </summary>
    public bool Handled { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="logLevel"></param>
    /// <param name="handled"></param>
    public ExceptionNotificationContext(Exception exception, LogLevel? logLevel = null, bool handled = true)
    {
        Exception = exception.NotNull(nameof(exception));
        LogLevel = logLevel ?? LogLevel.Error;
        Handled = handled;
    }
}

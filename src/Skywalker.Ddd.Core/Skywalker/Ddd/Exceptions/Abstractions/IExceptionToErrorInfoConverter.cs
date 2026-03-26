namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// Interface to convert exceptions to error information.
/// </summary>
public interface IExceptionToErrorInfoConverter
{
    /// <summary>
    /// Converts the given exception to an <see cref="ErrorInfo"/> object.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <param name="options">Options for conversion.</param>
    /// <returns>Error information.</returns>
    ErrorInfo Convert(Exception exception, ExceptionConvertOptions? options = null);
}

/// <summary>
/// Options for exception to error info conversion.
/// </summary>
public class ExceptionConvertOptions
{
    /// <summary>
    /// Gets or sets whether to include sensitive details like stack trace.
    /// </summary>
    public bool IncludeDetails { get; set; }

    /// <summary>
    /// Gets or sets whether to send all exception notifications to subscribers.
    /// </summary>
    public bool SendNotifications { get; set; } = true;
}


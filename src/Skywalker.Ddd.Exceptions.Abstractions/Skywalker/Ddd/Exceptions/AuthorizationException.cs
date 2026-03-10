using Microsoft.Extensions.Logging;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// Exception thrown when authorization fails.
/// </summary>
[Serializable]
public class AuthorizationException : SkywalkerException, IHasErrorCode, IHasLogLevel
{
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// Gets the log level for this exception.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;

    /// <summary>
    /// Creates a new <see cref="AuthorizationException"/> object.
    /// </summary>
    public AuthorizationException()
    {
        Code = "Skywalker:Authorization";
    }

    /// <summary>
    /// Creates a new <see cref="AuthorizationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    public AuthorizationException(string message)
        : base(message)
    {
        Code = "Skywalker:Authorization";
    }

    /// <summary>
    /// Creates a new <see cref="AuthorizationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public AuthorizationException(string message, Exception? innerException)
        : base(message, innerException)
    {
        Code = "Skywalker:Authorization";
    }

    /// <summary>
    /// Creates a new <see cref="AuthorizationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="code">Error code</param>
    public AuthorizationException(string message, string? code)
        : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Creates a new <see cref="AuthorizationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="code">Error code</param>
    /// <param name="innerException">Inner exception</param>
    public AuthorizationException(string message, string? code, Exception? innerException)
        : base(message, innerException)
    {
        Code = code;
    }

    /// <summary>
    /// Fluent API to set data on the exception.
    /// </summary>
    public AuthorizationException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}


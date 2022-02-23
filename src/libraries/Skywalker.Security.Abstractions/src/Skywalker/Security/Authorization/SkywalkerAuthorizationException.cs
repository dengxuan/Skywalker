using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Skywalker.Extensions.Exceptions;
using Skywalker.Logging;

namespace Skywalker.Security.Authorization;

/// <summary>
/// This exception is thrown on an unauthorized request.
/// </summary>
[Serializable]
public class SkywalkerAuthorizationException : SkywalkerException, IHasLogLevel, IHasErrorCode
{
    /// <summary>
    /// Severity of the exception.
    /// Default: Warn.
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// Error code.
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// Creates a new <see cref="SkywalkerAuthorizationException"/> object.
    /// </summary>
    public SkywalkerAuthorizationException()
    {
        LogLevel = LogLevel.Warning;
    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerAuthorizationException"/> object.
    /// </summary>
    public SkywalkerAuthorizationException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {

    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerAuthorizationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    public SkywalkerAuthorizationException(string? message)
        : base(message)
    {
        LogLevel = LogLevel.Warning;
    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerAuthorizationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public SkywalkerAuthorizationException(string? message, Exception innerException)
        : base(message, innerException)
    {
        LogLevel = LogLevel.Warning;
    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerAuthorizationException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="code">Exception code</param>
    /// <param name="innerException">Inner exception</param>
    public SkywalkerAuthorizationException(string? message = null, string? code = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
        LogLevel = LogLevel.Warning;
    }

    public SkywalkerAuthorizationException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}

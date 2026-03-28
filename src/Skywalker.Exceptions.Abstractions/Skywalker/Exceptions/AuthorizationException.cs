using Microsoft.Extensions.Logging;

namespace Skywalker.Exceptions;

[Serializable]
public class AuthorizationException : SkywalkerException, IHasErrorCode, IHasLogLevel
{
    public string? Code { get; }
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;

    public AuthorizationException()
    {
        Code = "Skywalker:Authorization";
    }

    public AuthorizationException(string message) : base(message)
    {
        Code = "Skywalker:Authorization";
    }

    public AuthorizationException(string message, Exception? innerException) : base(message, innerException)
    {
        Code = "Skywalker:Authorization";
    }

    public AuthorizationException(string message, string? code) : base(message)
    {
        Code = code;
    }

    public AuthorizationException(string message, string? code, Exception? innerException) : base(message, innerException)
    {
        Code = code;
    }

    public AuthorizationException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}

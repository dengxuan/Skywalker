namespace Skywalker.Exceptions;

public class UserFriendlyException : SkywalkerException, IHasErrorCode
{
    public string? Code { get; }

    public UserFriendlyException(string? code) : this(code, string.Empty) { }

    public UserFriendlyException(string? code, string message) : base(message)
    {
        Code = code;
    }

    public UserFriendlyException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}

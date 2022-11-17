namespace Skywalker.ExceptionHandler;

/// <summary>
/// 
/// </summary>
public class UserFriendlyException : SkywalkerException, IHasErrorCode
{
    /// <summary>
    /// 
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    public UserFriendlyException(string? code) : this(code, string.Empty)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    public UserFriendlyException(string? code, string message) : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public UserFriendlyException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}

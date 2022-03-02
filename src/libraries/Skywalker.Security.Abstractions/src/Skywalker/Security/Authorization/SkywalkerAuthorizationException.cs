using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Skywalker.Extensions.Exceptions;
using Skywalker.Logging;

namespace Skywalker.Security.Authorization;

/// <summary>
/// 当一个请求未授权时抛出此异常
/// </summary>
[Serializable]
public class SkywalkerAuthorizationException : SkywalkerException, IHasLogLevel, IHasErrorCode
{
    /// <summary>
    /// 异常的严重级别
    /// 默认: Warning
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// 错误码.
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// 创建一个 <see cref="SkywalkerAuthorizationException"/> 对象.
    /// </summary>
    public SkywalkerAuthorizationException()
    {
        LogLevel = LogLevel.Warning;
    }

    /// <summary>
    /// 创建一个 <see cref="SkywalkerAuthorizationException"/> 对象.
    /// <paramref name="serializationInfo">序列化信息</paramref>
    /// <paramref name="context">序列化流上下文</paramref>
    /// </summary>
    public SkywalkerAuthorizationException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {

    }

    /// <summary>
    /// 创建一个 <see cref="SkywalkerAuthorizationException"/> 对象.
    /// </summary>
    /// <param name="message">异常消息</param>
    public SkywalkerAuthorizationException(string? message)
        : base(message)
    {
        LogLevel = LogLevel.Warning;
    }

    /// <summary>
    /// 创建一个 <see cref="SkywalkerAuthorizationException"/> 对象.
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">内部异常</param>
    public SkywalkerAuthorizationException(string? message, Exception innerException)
        : base(message, innerException)
    {
        LogLevel = LogLevel.Warning;
    }

    /// <summary>
    /// 创建一个 <see cref="SkywalkerAuthorizationException"/> 对象.
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="code">错误码</param>
    /// <param name="innerException">内部异常</param>
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

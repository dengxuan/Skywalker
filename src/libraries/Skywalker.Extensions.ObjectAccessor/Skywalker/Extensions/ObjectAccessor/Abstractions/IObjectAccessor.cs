namespace Skywalker.Extensions.ObjectAccessor.Abstractions;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IObjectAccessor<out T>
{
    /// <summary>
    /// 
    /// </summary>
    T? Value { get; }
}

using Skywalker.Extensions.DependencyInjection.Abstractions;

namespace Skywalker.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ObjectAccessor<T> : IObjectAccessor<T>
{
    /// <summary>
    /// 
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ObjectAccessor() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public ObjectAccessor(T? value)
    {
        Value = value;
    }
}

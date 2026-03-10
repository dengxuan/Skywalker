using Skywalker.Extensions.ObjectAccessor.Abstractions;

namespace Skywalker.Extensions.ObjectAccessor;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class DefaultObjectAccessor<T> : IObjectAccessor<T>
{
    /// <summary>
    /// 
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public DefaultObjectAccessor(T value)
    {
        Value = value;
    }
}

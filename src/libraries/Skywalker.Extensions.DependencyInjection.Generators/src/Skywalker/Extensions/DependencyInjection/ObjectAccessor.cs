using Skywalker.Extensions.DependencyInjection.Abstractions;

namespace Skywalker.Extensions.DependencyInjection;

public sealed class ObjectAccessor<T> : IObjectAccessor<T>
{
    public T Value { get; set; }

    public ObjectAccessor(T obj)
    {
        Value = obj;
    }
}

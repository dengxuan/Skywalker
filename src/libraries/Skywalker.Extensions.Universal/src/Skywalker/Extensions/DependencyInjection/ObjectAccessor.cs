namespace Microsoft.Extensions.DependencyInjection;

public class ObjectAccessor<T> : IObjectAccessor<T>
{
    public T Value { get; set; }

    public ObjectAccessor(T obj)
    {
        Value = obj;
    }
}

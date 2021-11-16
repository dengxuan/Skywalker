using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.DependencyInjection
{
    public class ObjectAccessor<T> : IObjectAccessor<T>
    {
        public T Value { get; set; }

        public ObjectAccessor([MaybeNull] T obj)
        {
            Value = obj;
        }
    }
}

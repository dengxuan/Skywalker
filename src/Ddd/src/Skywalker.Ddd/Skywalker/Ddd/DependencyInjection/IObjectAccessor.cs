using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.DependencyInjection
{
    public interface IObjectAccessor<out T>
    {
        [MaybeNull]
        T Value { get; }
    }
}

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IObjectAccessor<out T>
    {
        [MaybeNull]
        T Value { get; }
    }
}

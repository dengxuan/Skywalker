namespace Microsoft.Extensions.DependencyInjection;

public interface IObjectAccessor<out T>
{

    T Value { get; }
}

namespace Skywalker.Extensions.DependencyInjection.Abstractions;

public interface IObjectAccessor<out T>
{

    T Value { get; }
}

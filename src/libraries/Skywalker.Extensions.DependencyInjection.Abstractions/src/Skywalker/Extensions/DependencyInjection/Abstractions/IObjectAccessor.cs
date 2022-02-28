namespace Skywalker.Extensions.DependencyInjection.Abstractions;

public interface IObjectAccessor<out T>: ISingletonDependency
{

    T Value { get; }
}

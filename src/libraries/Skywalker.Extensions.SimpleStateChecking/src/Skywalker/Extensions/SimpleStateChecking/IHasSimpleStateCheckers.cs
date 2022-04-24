using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Extensions.SimpleStateChecking;

public interface IHasSimpleStateCheckers<TState>: ITransientDependency where TState : IHasSimpleStateCheckers<TState>
{
    List<ISimpleStateChecker<TState>> StateCheckers { get; }
}

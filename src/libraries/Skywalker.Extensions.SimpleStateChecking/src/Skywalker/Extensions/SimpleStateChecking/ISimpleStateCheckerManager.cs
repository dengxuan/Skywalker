using Skywalker.Extensions.DependencyInjection;
namespace Skywalker.Extensions.SimpleStateChecking;

public interface ISimpleStateCheckerManager<TState> : ISingletonDependency where TState : IHasSimpleStateCheckers<TState>
{
    Task<bool> IsEnabledAsync(TState state);

    Task<SimpleStateCheckerResult<TState>> IsEnabledAsync(TState[] states);
}

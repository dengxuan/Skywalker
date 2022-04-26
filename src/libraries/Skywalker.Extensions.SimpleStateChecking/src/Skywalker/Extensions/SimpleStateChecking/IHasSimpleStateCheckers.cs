namespace Skywalker.Extensions.SimpleStateChecking;

public interface IHasSimpleStateCheckers<TState> where TState : IHasSimpleStateCheckers<TState>
{
    List<ISimpleStateChecker<TState>> StateCheckers { get; }
}

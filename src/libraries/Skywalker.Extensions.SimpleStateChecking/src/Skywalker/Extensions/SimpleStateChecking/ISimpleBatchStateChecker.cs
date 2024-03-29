﻿namespace Skywalker.Extensions.SimpleStateChecking;

public interface ISimpleBatchStateChecker<TState> : ISimpleStateChecker<TState> where TState : IHasSimpleStateCheckers<TState>
{
    Task<SimpleStateCheckerResult<TState>> IsEnabledAsync(SimpleBatchStateCheckerContext<TState> context);
}

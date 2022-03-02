using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Extensions.DependencyInjection;
using Skywalker.Extensions.DependencyInjection.Abstractions;

namespace Skywalker.Extensions.SimpleStateChecking;

[TransientDependency]
public class SimpleStateCheckerManager<TState> : ISimpleStateCheckerManager<TState> where TState : IHasSimpleStateCheckers<TState>
{
    protected IServiceProvider ServiceProvider { get; }

    protected SimpleStateCheckerOptions<TState> Options { get; }

    public SimpleStateCheckerManager(IServiceProvider serviceProvider, IOptions<SimpleStateCheckerOptions<TState>> options)
    {
        ServiceProvider = serviceProvider;
        Options = options.Value;
    }

    public virtual async Task<bool> IsEnabledAsync(TState state)
    {
        return await InternalIsEnabledAsync(state, true);
    }

    public virtual async Task<SimpleStateCheckerResult<TState>> IsEnabledAsync(TState[] states)
    {
        var result = new SimpleStateCheckerResult<TState>(states);

        using (var scope = ServiceProvider.CreateScope())
        {
            var batchStateCheckers = states.SelectMany(x => x.StateCheckers)
                .Where(x => x is ISimpleBatchStateChecker<TState>)
                .Cast<ISimpleBatchStateChecker<TState>>()
                .GroupBy(x => x)
                .Select(x => x.Key);

            foreach (var stateChecker in batchStateCheckers)
            {
                var cachedServiceProvider = scope.ServiceProvider.GetRequiredService<ICachedServiceProvider>();
                var stateCheckers = states.Where(x => x.StateCheckers.Contains(stateChecker)).ToArray();
                var context = new SimpleBatchStateCheckerContext<TState>(cachedServiceProvider, stateCheckers);

                foreach (var x in await stateChecker.IsEnabledAsync(context))
                {
                    result[x.Key] = x.Value;
                }

                if (result.Values.All(x => !x))
                {
                    return result;
                }
            }

            foreach (ISimpleBatchStateChecker<TState> globalStateChecker in Options.GlobalStateCheckers
                .Where(x => typeof(ISimpleBatchStateChecker<TState>).IsAssignableFrom(x))
                .Select(x => ServiceProvider.GetRequiredService(x)))
            {
                var context = new SimpleBatchStateCheckerContext<TState>(
                    scope.ServiceProvider.GetRequiredService<ICachedServiceProvider>(),
                    states.Where(x => result.Any(y => y.Key.Equals(x) && y.Value)).ToArray());

                foreach (var x in await globalStateChecker.IsEnabledAsync(context))
                {
                    result[x.Key] = x.Value;
                }
            }

            foreach (var state in states)
            {
                if (result[state])
                {
                    result[state] = await InternalIsEnabledAsync(state, false);
                }
            }

            return result;
        }
    }

    protected virtual async Task<bool> InternalIsEnabledAsync(TState state, bool useBatchChecker)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var cachedServiceProvider = scope.ServiceProvider.GetRequiredService<ICachedServiceProvider>();
            var context = new SimpleStateCheckerContext<TState>(cachedServiceProvider, state);

            foreach (var provider in state.StateCheckers.WhereIf(!useBatchChecker, x => x is not ISimpleBatchStateChecker<TState>))
            {
                if (!await provider.IsEnabledAsync(context))
                {
                    return false;
                }
            }

            foreach (ISimpleStateChecker<TState> provider in Options.GlobalStateCheckers
                .WhereIf(!useBatchChecker, x => !typeof(ISimpleBatchStateChecker<TState>).IsAssignableFrom(x))
                .Select(x => ServiceProvider.GetRequiredService(x)))
            {
                if (!await provider.IsEnabledAsync(context))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

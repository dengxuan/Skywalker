using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Extensions.SimpleStateChecking;

namespace Skywalker.Extensions.SimpleStateChecking.Tests;

public class TestState : IHasSimpleStateCheckers<TestState>
{
    public string Name { get; set; } = string.Empty;
    public List<ISimpleStateChecker<TestState>> StateCheckers { get; } = new();
}

public class SimpleStateCheckerManagerTests
{
    private readonly IServiceProvider _serviceProvider;

    public SimpleStateCheckerManagerTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IServiceProvider>(sp => sp);
        _serviceProvider = services.BuildServiceProvider();
    }

    private SimpleStateCheckerManager<TestState> CreateManager(Action<SimpleStateCheckerOptions<TestState>>? configure = null)
    {
        var options = new SimpleStateCheckerOptions<TestState>();
        configure?.Invoke(options);
        return new SimpleStateCheckerManager<TestState>(_serviceProvider, Options.Create(options));
    }

    [Fact]
    public async Task IsEnabledAsync_NoCheckers_ReturnsTrue()
    {
        var manager = CreateManager();
        var state = new TestState { Name = "test" };

        var result = await manager.IsEnabledAsync(state);

        Assert.True(result);
    }

    [Fact]
    public async Task IsEnabledAsync_AllCheckersReturnTrue_ReturnsTrue()
    {
        var manager = CreateManager();
        var checker = Substitute.For<ISimpleStateChecker<TestState>>();
        checker.IsEnabledAsync(Arg.Any<SimpleStateCheckerContext<TestState>>()).Returns(true);

        var state = new TestState { Name = "test" };
        state.StateCheckers.Add(checker);

        var result = await manager.IsEnabledAsync(state);

        Assert.True(result);
    }

    [Fact]
    public async Task IsEnabledAsync_AnyCheckerReturnsFalse_ReturnsFalse()
    {
        var manager = CreateManager();
        var checker1 = Substitute.For<ISimpleStateChecker<TestState>>();
        checker1.IsEnabledAsync(Arg.Any<SimpleStateCheckerContext<TestState>>()).Returns(true);

        var checker2 = Substitute.For<ISimpleStateChecker<TestState>>();
        checker2.IsEnabledAsync(Arg.Any<SimpleStateCheckerContext<TestState>>()).Returns(false);

        var state = new TestState { Name = "test" };
        state.StateCheckers.Add(checker1);
        state.StateCheckers.Add(checker2);

        var result = await manager.IsEnabledAsync(state);

        Assert.False(result);
    }

    [Fact]
    public async Task IsEnabledAsync_ShortCircuits_OnFirstFalse()
    {
        var manager = CreateManager();
        var checker1 = Substitute.For<ISimpleStateChecker<TestState>>();
        checker1.IsEnabledAsync(Arg.Any<SimpleStateCheckerContext<TestState>>()).Returns(false);

        var checker2 = Substitute.For<ISimpleStateChecker<TestState>>();

        var state = new TestState { Name = "test" };
        state.StateCheckers.Add(checker1);
        state.StateCheckers.Add(checker2);

        var result = await manager.IsEnabledAsync(state);

        Assert.False(result);
        await checker2.DidNotReceive().IsEnabledAsync(Arg.Any<SimpleStateCheckerContext<TestState>>());
    }

    [Fact]
    public async Task IsEnabledAsync_BatchStates_ReturnsResultForEach()
    {
        var manager = CreateManager();

        var state1 = new TestState { Name = "s1" };
        var state2 = new TestState { Name = "s2" };

        var checker = Substitute.For<ISimpleStateChecker<TestState>>();
        checker.IsEnabledAsync(Arg.Any<SimpleStateCheckerContext<TestState>>())
            .Returns(callInfo =>
            {
                var ctx = callInfo.Arg<SimpleStateCheckerContext<TestState>>();
                return Task.FromResult(ctx.State.Name == "s1");
            });

        state1.StateCheckers.Add(checker);
        state2.StateCheckers.Add(checker);

        var result = await manager.IsEnabledAsync(new[] { state1, state2 });

        Assert.True(result[state1]);
        Assert.False(result[state2]);
    }

    [Fact]
    public async Task IsEnabledAsync_BatchStates_EmptyCheckers_AllTrue()
    {
        var manager = CreateManager();
        var states = new[]
        {
            new TestState { Name = "s1" },
            new TestState { Name = "s2" },
            new TestState { Name = "s3" },
        };

        var result = await manager.IsEnabledAsync(states);

        Assert.All(result.Values, v => Assert.True(v));
    }
}

public class SimpleStateCheckerResultTests
{
    [Fact]
    public void Constructor_InitializesAllStatesWithValue()
    {
        var states = new[]
        {
            new TestState { Name = "s1" },
            new TestState { Name = "s2" },
        };

        var result = new SimpleStateCheckerResult<TestState>(states, true);

        Assert.True(result[states[0]]);
        Assert.True(result[states[1]]);
    }

    [Fact]
    public void Constructor_InitializesWithFalse()
    {
        var states = new[] { new TestState { Name = "s1" } };

        var result = new SimpleStateCheckerResult<TestState>(states, false);

        Assert.False(result[states[0]]);
    }

    [Fact]
    public void DefaultConstructor_IsEmpty()
    {
        var result = new SimpleStateCheckerResult<TestState>();

        Assert.Empty(result);
    }
}

public class SimpleStateCheckerContextTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var sp = Substitute.For<IServiceProvider>();
        var state = new TestState { Name = "test" };

        var context = new SimpleStateCheckerContext<TestState>(sp, state);

        Assert.Same(sp, context.ServiceProvider);
        Assert.Same(state, context.State);
    }
}

public class SimpleBatchStateCheckerContextTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var sp = Substitute.For<IServiceProvider>();
        var states = new[] { new TestState { Name = "s1" }, new TestState { Name = "s2" } };

        var context = new SimpleBatchStateCheckerContext<TestState>(sp, states);

        Assert.Same(sp, context.ServiceProvider);
        Assert.Equal(2, context.States.Length);
    }
}

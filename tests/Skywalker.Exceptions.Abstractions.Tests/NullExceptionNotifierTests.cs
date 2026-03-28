using Skywalker.Exceptions;

namespace Skywalker.Exceptions.Abstractions.Tests;

public class NullExceptionNotifierTests
{
    [Fact]
    public void Instance_IsSingleton()
    {
        Assert.Same(NullExceptionNotifier.Instance, NullExceptionNotifier.Instance);
    }

    [Fact]
    public async Task NotifyAsync_DoesNotThrow()
    {
        var ctx = new ExceptionNotificationContext(new InvalidOperationException("test"));
        await NullExceptionNotifier.Instance.NotifyAsync(ctx);
    }
}

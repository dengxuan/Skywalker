using Skywalker.Sms;
using Skywalker.Sms.Abstractions;
using Xunit;

namespace Skywalker.Sms.Tests;

public class NullSmsSenderTests
{
    [Fact]
    public void NullSmsSender_HasLogger()
    {
        var sender = new NullSmsSender();
        Assert.NotNull(sender.Logger);
    }

    [Fact]
    public async Task SendAsync_DoesNotThrow()
    {
        var sender = new NullSmsSender();
        var message = new SmsMessage("1234567890", "test message");
        await sender.SendAsync(message);
    }
}

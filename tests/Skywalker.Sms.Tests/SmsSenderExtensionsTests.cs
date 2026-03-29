using NSubstitute;
using Skywalker.Sms;
using Skywalker.Sms.Abstractions;
using Xunit;

namespace Skywalker.Sms.Tests;

public class SmsSenderExtensionsTests
{
    [Fact]
    public async Task SendAsync_CreatesMessageAndDelegates()
    {
        var sender = Substitute.For<ISmsSender>();

        await sender.SendAsync("13800138000", "hello");

        await sender.Received(1).SendAsync(Arg.Is<SmsMessage>(m =>
            m.PhoneNumber == "13800138000" && m.Text == "hello"));
    }
}

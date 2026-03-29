using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Extensions.Emailing;
using Xunit;

namespace Skywalker.Extensions.Emailing.Tests;

public class NullEmailSenderTests
{
    private static IOptions<SmtpEmailSenderConfiguration> CreateOptions(
        string fromAddress = "noreply@test.com", string fromName = "Test")
    {
        var opts = Substitute.For<IOptions<SmtpEmailSenderConfiguration>>();
        opts.Value.Returns(new SmtpEmailSenderConfiguration
        {
            DefaultFromAddress = fromAddress,
            DefaultFromDisplayName = fromName
        });
        return opts;
    }

    [Fact]
    public async Task SendAsync_WithStringParams_DoesNotThrow()
    {
        var sender = new NullEmailSender(CreateOptions());

        await sender.SendAsync("to@test.com", "Subject", "Body");
    }

    [Fact]
    public async Task SendAsync_WithFromAndTo_DoesNotThrow()
    {
        var sender = new NullEmailSender(CreateOptions());

        await sender.SendAsync("from@test.com", "to@test.com", "Subject", "Body");
    }

    [Fact]
    public async Task QueueAsync_DelegatesToSendAsync()
    {
        var sender = new NullEmailSender(CreateOptions());

        // QueueAsync should not throw — it delegates to SendAsync
        await sender.QueueAsync("to@test.com", "Subject", "Body");
    }

    [Fact]
    public async Task SendAsync_NormalizesMailMessage_SetsFrom()
    {
        var sender = new NullEmailSender(CreateOptions("default@test.com", "Default"));
        var mail = new MailMessage { Subject = "Test" };
        mail.To.Add("to@test.com");

        await sender.SendAsync(mail, normalize: true);

        Assert.Equal("default@test.com", mail.From!.Address);
    }

    [Fact]
    public async Task SendAsync_NormalizesMailMessage_SetsEncodings()
    {
        var sender = new NullEmailSender(CreateOptions());
        var mail = new MailMessage { Subject = "Test" };
        mail.To.Add("to@test.com");

        await sender.SendAsync(mail, normalize: true);

        Assert.Equal(Encoding.UTF8, mail.HeadersEncoding);
        Assert.Equal(Encoding.UTF8, mail.SubjectEncoding);
        Assert.Equal(Encoding.UTF8, mail.BodyEncoding);
    }

    [Fact]
    public async Task SendAsync_SkipsNormalize_WhenFalse()
    {
        var sender = new NullEmailSender(CreateOptions());
        var mail = new MailMessage("explicit@test.com", "to@test.com", "Subject", "Body");

        await sender.SendAsync(mail, normalize: false);

        // From should remain as explicitly set
        Assert.Equal("explicit@test.com", mail.From!.Address);
        // Encodings should NOT be set since normalize=false
        Assert.Null(mail.HeadersEncoding);
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Skywalker.Sms.Abstractions;

namespace Skywalker.Sms;

public class NullSmsSender : ISmsSender
{
    public ILogger<NullSmsSender> Logger { get; set; }

    public NullSmsSender()
    {
        Logger = NullLogger<NullSmsSender>.Instance;
    }

    public Task SendAsync(SmsMessage smsMessage)
    {
        Logger.LogWarning($"SMS Sending was not implemented! Using {nameof(NullSmsSender)}:");

        Logger.LogWarning("Phone Number : {PhoneNumber}", smsMessage.PhoneNumber);
        Logger.LogWarning("SMS Text     : {Text}", smsMessage.Text);

        return Task.CompletedTask;
    }
}

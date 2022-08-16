using Skywalker.Sms.Abstractions;

namespace Skywalker.Sms;

public static class SmsSenderExtensions
{
    public static Task SendAsync(this ISmsSender smsSender, string phoneNumber, string text)
    {
        return smsSender.SendAsync(new SmsMessage(phoneNumber, text));
    }
}

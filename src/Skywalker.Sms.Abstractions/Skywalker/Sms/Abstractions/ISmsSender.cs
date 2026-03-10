using System.Threading.Tasks;

namespace Skywalker.Sms.Abstractions;

public interface ISmsSender
{
    Task SendAsync(SmsMessage smsMessage);
}

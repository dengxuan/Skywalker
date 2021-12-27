namespace Skywalker.Extensions.Exceptions.Abstractions;

public interface IExceptionNotifier
{
    Task NotifyAsync(ExceptionNotificationContext context);
}

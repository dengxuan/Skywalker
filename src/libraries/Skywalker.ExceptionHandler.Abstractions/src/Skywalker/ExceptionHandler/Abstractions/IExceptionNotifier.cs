namespace Skywalker.ExceptionHandler.Abstractions;

public interface IExceptionNotifier
{
    Task NotifyAsync(ExceptionNotificationContext context);
}

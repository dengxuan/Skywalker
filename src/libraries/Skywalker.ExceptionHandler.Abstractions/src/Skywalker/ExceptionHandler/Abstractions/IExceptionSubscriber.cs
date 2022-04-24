namespace Skywalker.ExceptionHandler.Abstractions;

public interface IExceptionSubscriber
{
    Task HandleAsync(ExceptionNotificationContext context);
}

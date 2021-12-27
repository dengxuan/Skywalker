namespace Skywalker.Extensions.Exceptions.Abstractions;

public interface IExceptionSubscriber
{
    Task HandleAsync(ExceptionNotificationContext context);
}

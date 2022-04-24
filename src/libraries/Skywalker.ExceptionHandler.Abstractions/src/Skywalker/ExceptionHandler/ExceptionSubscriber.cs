using Skywalker.ExceptionHandler.Abstractions;

namespace Skywalker.ExceptionHandler;

public abstract class ExceptionSubscriber : IExceptionSubscriber
{
    public abstract Task HandleAsync(ExceptionNotificationContext context);
}

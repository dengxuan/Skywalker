using Skywalker.Exceptions.Abstractions;

namespace Skywalker.Exceptions;

public abstract class ExceptionSubscriber : IExceptionSubscriber
{
    public abstract Task HandleAsync(ExceptionNotificationContext context);
}

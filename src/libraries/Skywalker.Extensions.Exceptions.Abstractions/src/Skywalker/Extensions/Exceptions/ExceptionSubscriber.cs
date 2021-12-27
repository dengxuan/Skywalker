using Skywalker.Extensions.Exceptions.Abstractions;

namespace Skywalker.Extensions.Exceptions;

public abstract class ExceptionSubscriber : IExceptionSubscriber
{
    public abstract Task HandleAsync(ExceptionNotificationContext context);
}

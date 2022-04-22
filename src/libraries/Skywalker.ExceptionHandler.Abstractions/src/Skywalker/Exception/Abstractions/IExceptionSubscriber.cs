using Skywalker.Exceptions;

namespace Skywalker.Exceptions.Abstractions;

public interface IExceptionSubscriber
{
    Task HandleAsync(ExceptionNotificationContext context);
}

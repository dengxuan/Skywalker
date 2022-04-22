using Skywalker.Exceptions;

namespace Skywalker.Exceptions.Abstractions;

public interface IExceptionNotifier
{
    Task NotifyAsync(ExceptionNotificationContext context);
}

using Skywalker.ExceptionHandler.Abstractions;

namespace Skywalker.ExceptionHandler;

public class NullExceptionNotifier : IExceptionNotifier
{
    public Task NotifyAsync(ExceptionNotificationContext context)
    {
        return Task.CompletedTask;
    }
}

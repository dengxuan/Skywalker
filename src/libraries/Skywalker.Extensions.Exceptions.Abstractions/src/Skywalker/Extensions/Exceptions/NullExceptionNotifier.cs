using Skywalker.Extensions.Exceptions.Abstractions;

namespace Skywalker.Extensions.Exceptions;

public class NullExceptionNotifier : IExceptionNotifier
{
    public static NullExceptionNotifier Instance { get; } = new NullExceptionNotifier();

    private NullExceptionNotifier()
    {

    }

    public Task NotifyAsync(ExceptionNotificationContext context)
    {
        return Task.CompletedTask;
    }
}

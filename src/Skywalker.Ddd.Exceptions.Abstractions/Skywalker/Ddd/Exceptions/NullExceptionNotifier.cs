using Skywalker.Ddd.Exceptions.Abstractions;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// 
/// </summary>
public class NullExceptionNotifier : IExceptionNotifier
{
    /// <summary>
    /// 
    /// </summary>
    public static NullExceptionNotifier Instance { get; } = new NullExceptionNotifier();

    private NullExceptionNotifier()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task NotifyAsync(ExceptionNotificationContext context)
    {
        return Task.CompletedTask;
    }
}

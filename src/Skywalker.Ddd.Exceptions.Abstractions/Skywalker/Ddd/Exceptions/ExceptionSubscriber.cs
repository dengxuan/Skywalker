using Skywalker.Ddd.Exceptions.Abstractions;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// 
/// </summary>
public abstract class ExceptionSubscriber : IExceptionSubscriber
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract Task HandleAsync(ExceptionNotificationContext context);
}

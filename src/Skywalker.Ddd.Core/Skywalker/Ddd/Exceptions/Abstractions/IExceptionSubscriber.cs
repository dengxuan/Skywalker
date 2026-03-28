namespace Skywalker.Ddd.Exceptions.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IExceptionSubscriber
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task HandleAsync(ExceptionNotificationContext context);
}

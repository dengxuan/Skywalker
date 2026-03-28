namespace Skywalker.Ddd.Exceptions.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IExceptionNotifier
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task NotifyAsync(ExceptionNotificationContext context);
}

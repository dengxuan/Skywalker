using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Skywalker.Ddd.Exceptions.Abstractions;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// 
/// </summary>
public class ExceptionNotifier : IExceptionNotifier
{
    /// <summary>
    /// 
    /// </summary>
    public ILogger<ExceptionNotifier> Logger { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected IServiceScopeFactory ServiceScopeFactory { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceScopeFactory"></param>
    public ExceptionNotifier(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Logger = NullLogger<ExceptionNotifier>.Instance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual async Task NotifyAsync(ExceptionNotificationContext context)
    {
        context.NotNull(nameof(context));

        using var scope = ServiceScopeFactory.CreateScope();
        var exceptionSubscribers = scope.ServiceProvider
            .GetServices<IExceptionSubscriber>();

        foreach (var exceptionSubscriber in exceptionSubscribers)
        {
            try
            {
                await exceptionSubscriber.HandleAsync(context);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "Exception subscriber of type {AssemblyQualifiedName} has thrown an exception!", exceptionSubscriber.GetType().AssemblyQualifiedName);
            }
        }
    }
}

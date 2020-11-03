using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.ExceptionHandling
{
    public class ExceptionNotifier : IExceptionNotifier
    {
        public ILogger<ExceptionNotifier> Logger { get; set; }

        protected IServiceScopeFactory ServiceScopeFactory { get; }

        public ExceptionNotifier(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory;
            Logger = NullLogger<ExceptionNotifier>.Instance;
        }

        public virtual async Task NotifyAsync([NotNull] ExceptionNotificationContext context)
        {
            Check.NotNull(context, nameof(context));

            using (var scope = ServiceScopeFactory.CreateScope())
            {
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
    }
}

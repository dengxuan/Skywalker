using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.ExceptionHandling
{
    public class NullExceptionNotifier : IExceptionNotifier
    {
        public static NullExceptionNotifier Instance { get; } = new NullExceptionNotifier();

        private NullExceptionNotifier()
        {
            
        }

        public Task NotifyAsync([NotNull] ExceptionNotificationContext context)
        {
            return Task.CompletedTask;
        }
    }
}
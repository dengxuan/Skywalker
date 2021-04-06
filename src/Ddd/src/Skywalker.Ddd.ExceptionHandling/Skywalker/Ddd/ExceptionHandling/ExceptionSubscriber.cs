using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.ExceptionHandling
{
    public abstract class ExceptionSubscriber : IExceptionSubscriber
    {
        public abstract Task HandleAsync([NotNull] ExceptionNotificationContext context);
    }
}
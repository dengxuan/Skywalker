using System.Threading.Tasks;

namespace Skywalker.ExceptionHandling
{
    public abstract class ExceptionSubscriber : IExceptionSubscriber
    {
        public abstract Task HandleAsync(ExceptionNotificationContext context);
    }
}
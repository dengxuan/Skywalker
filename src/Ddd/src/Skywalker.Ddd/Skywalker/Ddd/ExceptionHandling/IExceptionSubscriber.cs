using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.ExceptionHandling
{
    public interface IExceptionSubscriber
    {
        Task HandleAsync([NotNull] ExceptionNotificationContext context);
    }
}

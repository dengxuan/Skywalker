using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.Ddd.ExceptionHandling
{
    public interface IExceptionSubscriber
    {
        Task HandleAsync([NotNull] ExceptionNotificationContext context);
    }
}

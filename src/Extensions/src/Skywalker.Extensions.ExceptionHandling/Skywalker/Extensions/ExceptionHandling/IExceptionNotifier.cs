using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.ExceptionHandling
{
    public interface IExceptionNotifier
    {
        Task NotifyAsync([NotNull] ExceptionNotificationContext context);
    }
}
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.Ddd.ExceptionHandling
{
    public interface IExceptionNotifier
    {
        Task NotifyAsync([NotNull] ExceptionNotificationContext context);
    }
}
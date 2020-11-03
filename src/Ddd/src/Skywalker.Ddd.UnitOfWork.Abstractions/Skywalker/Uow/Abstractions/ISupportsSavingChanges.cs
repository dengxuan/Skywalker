using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Uow.Abstractions
{
    public interface ISupportsSavingChanges
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
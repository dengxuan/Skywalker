using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface ISupportsSavingChanges
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
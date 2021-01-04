using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface ISupportsRollback
    {
        void Rollback();

        Task RollbackAsync(CancellationToken cancellationToken);
    }
}
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Uow.Abstractions
{
    public interface ISupportsRollback
    {
        void Rollback();

        Task RollbackAsync(CancellationToken cancellationToken);
    }
}
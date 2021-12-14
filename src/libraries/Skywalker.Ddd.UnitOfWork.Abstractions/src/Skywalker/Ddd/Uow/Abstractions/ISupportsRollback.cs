namespace Skywalker.Ddd.Uow.Abstractions;

public interface ISupportsRollback
{
    void Rollback();

    Task RollbackAsync(CancellationToken cancellationToken);
}

namespace Skywalker.Ddd.Uow.Abstractions;

public interface ISupportsSavingChanges
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

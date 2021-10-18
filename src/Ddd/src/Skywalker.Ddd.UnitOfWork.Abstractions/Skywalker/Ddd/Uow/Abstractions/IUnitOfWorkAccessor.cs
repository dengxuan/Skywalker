namespace Skywalker.Ddd.Uow.Abstractions;

public interface IUnitOfWorkAccessor
{
    IUnitOfWork? UnitOfWork { get; }

    void SetUnitOfWork( IUnitOfWork? unitOfWork);
}

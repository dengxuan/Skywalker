namespace Skywalker.Ddd.Uow.Abstractions;

public interface IUnitOfWorkManagerAccessor
{
    IUnitOfWorkManager UnitOfWorkManager { get; }
}

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface IUnitOfWorkManagerAccessor
    {
        IUnitOfWorkManager UnitOfWorkManager { get; }
    }
}

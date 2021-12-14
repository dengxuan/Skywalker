namespace Skywalker.Ddd.Uow.Abstractions;

public interface IUnitOfWorkManager
{
    IUnitOfWork? Current { get; }

    IUnitOfWork Begin(AbpUnitOfWorkOptions options, bool requiresNew = false);

    IUnitOfWork Reserve(string reservationName, bool requiresNew = false);

    void BeginReserved(string reservationName, AbpUnitOfWorkOptions options);

    bool TryBeginReserved(string reservationName, AbpUnitOfWorkOptions options);
}

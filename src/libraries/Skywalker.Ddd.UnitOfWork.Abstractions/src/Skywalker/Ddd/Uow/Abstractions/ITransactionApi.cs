namespace Skywalker.Ddd.Uow.Abstractions;

public interface ITransactionApi : IDisposable
{
    Task CommitAsync();
}

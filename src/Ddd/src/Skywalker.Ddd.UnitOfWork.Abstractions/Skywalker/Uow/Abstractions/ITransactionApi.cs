using System;
using System.Threading.Tasks;

namespace Skywalker.Uow.Abstractions
{
    public interface ITransactionApi : IDisposable
    {
        Task CommitAsync();
    }
}
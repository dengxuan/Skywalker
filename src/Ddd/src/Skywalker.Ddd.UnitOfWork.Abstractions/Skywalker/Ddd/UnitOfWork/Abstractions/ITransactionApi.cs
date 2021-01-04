using System;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface ITransactionApi : IDisposable
    {
        Task CommitAsync();
    }
}
using System.Data;

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface IAbpUnitOfWorkOptions
    {
        bool IsTransactional { get; }

        IsolationLevel? IsolationLevel { get; }

        /// <summary>
        /// Milliseconds
        /// </summary>
        int? Timeout { get; }
    }
}

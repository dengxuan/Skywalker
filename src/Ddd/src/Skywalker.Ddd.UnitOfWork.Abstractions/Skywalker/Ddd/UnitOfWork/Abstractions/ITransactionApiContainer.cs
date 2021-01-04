using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface ITransactionApiContainer
    {
        ITransactionApi? FindTransactionApi([NotNull] string key);

        void AddTransactionApi([NotNull] string key, [NotNull] ITransactionApi api);

        ITransactionApi GetOrAddTransactionApi([NotNull] string key, [NotNull] Func<ITransactionApi> factory);
    }
}
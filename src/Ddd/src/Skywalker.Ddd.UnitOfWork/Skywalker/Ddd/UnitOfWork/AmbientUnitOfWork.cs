using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.UnitOfWork.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Skywalker.Ddd.UnitOfWork
{
    public class AmbientUnitOfWork : IAmbientUnitOfWork
    {
        public IUnitOfWork? UnitOfWork => _currentUow.Value;

        private readonly AsyncLocal<IUnitOfWork?> _currentUow;

        public AmbientUnitOfWork()
        {
            _currentUow = new AsyncLocal<IUnitOfWork?>();
        }

        public void SetUnitOfWork([MaybeNull] IUnitOfWork? unitOfWork)
        {
            _currentUow.Value = unitOfWork;
        }
    }
}
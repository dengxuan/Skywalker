using Microsoft.Extensions.DependencyInjection;
using Skywalker.Uow.Abstractions;
using System.Threading;

namespace Skywalker.Uow
{
    public class AmbientUnitOfWork : IAmbientUnitOfWork, ISingletonDependency
    {
        public IUnitOfWork UnitOfWork => _currentUow.Value;

        private readonly AsyncLocal<IUnitOfWork> _currentUow;

        public AmbientUnitOfWork()
        {
            _currentUow = new AsyncLocal<IUnitOfWork>();
        }

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            _currentUow.Value = unitOfWork;
        }
    }
}
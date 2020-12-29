using Skywalker.Uow.Abstractions;

namespace Skywalker.Ddd.EntityFrameworkCore
{
    public class SkywalkerEfCoreDbContextInitializationContext
    {
        public IUnitOfWork UnitOfWork { get; }

        public SkywalkerEfCoreDbContextInitializationContext(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}

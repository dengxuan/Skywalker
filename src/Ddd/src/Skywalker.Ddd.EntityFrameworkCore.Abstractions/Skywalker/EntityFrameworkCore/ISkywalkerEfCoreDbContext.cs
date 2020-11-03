namespace Skywalker.EntityFrameworkCore
{
    public interface ISkywalkerEfCoreDbContext : IEfCoreDbContext
    {
        void Initialize();
    }
}
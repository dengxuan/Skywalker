namespace Skywalker.Ddd.EntityFrameworkCore
{
    public interface ISkywalkerEfCoreDbContext
    {
        void Initialize(SkywalkerEfCoreDbContextInitializationContext contextInitializationContext);
    }
}

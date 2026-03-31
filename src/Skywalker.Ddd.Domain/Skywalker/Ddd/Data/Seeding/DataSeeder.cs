using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Data.Seeding;

public class DataSeeder : IDataSeeder
{
    protected IServiceScopeFactory ServiceScopeFactory { get; }
    protected SkywalkerDataSeedOptions Options { get; }

    public DataSeeder(IOptions<SkywalkerDataSeedOptions> options, IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Options = options.Value;
    }

    public virtual async Task SeedAsync(DataSeedContext context)
    {
        var unitOfWorkManager = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        using var uow = unitOfWorkManager.Begin(isTransactional: true);

        // 从 UoW 的 ServiceProvider 解析 DataSeedContributor，确保使用同一个 DbContext
        foreach (var contributorType in Options.Contributors)
        {
            var contributor = (IDataSeedContributor)uow.ServiceProvider!.GetRequiredService(contributorType);
            await contributor.SeedAsync(context);
        }

        await uow.CompleteAsync();
    }
}

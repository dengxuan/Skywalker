using Microsoft.Extensions.Caching.Memory;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Permissions;
using Skywalker.Sample.Website.Application;
using Skywalker.Sample.Website.Domain;

namespace Skywalker.Sample.Website.Web.Infrastructure;

public static class DataSeeder
{
    public static readonly Guid KeyboardId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid MouseId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static async Task SeedAsync(IServiceProvider services)
    {
        // 权限授予：admin 角色持有商品管理权限（写入 InMemoryPermissionValidator 的缓存）
        var grants = new Dictionary<string, HashSet<(string, string)>>
        {
            [WebsitePermissions.ManageProducts] = [("R", "admin")],
        };
        services.GetRequiredService<IMemoryCache>().Set(InMemoryPermissionValidator.GrantsCacheKey, grants);

        // 商品种子：仓储必须在工作单元内使用，启动期手动开启 UoW
        using var scope = services.CreateScope();
        var unitOfWorkManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        using var unitOfWork = unitOfWorkManager.Begin(new UnitOfWorkOptions());

        var products = scope.ServiceProvider.GetRequiredService<IRepository<Product, Guid>>();
        await products.InsertAsync(new Product(KeyboardId, "机械键盘", 399m, 100));
        await products.InsertAsync(new Product(MouseId, "无线鼠标", 129m, 200));

        await unitOfWork.CompleteAsync();
    }
}

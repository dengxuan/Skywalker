using Skywalker.Caching.Abstractions;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.DependencyInjection;
using Skywalker.Permissions.Abstractions;
using Skywalker.Localization;
using Skywalker.Sample.Website.Domain;
using Skywalker.Security.Claims;

namespace Skywalker.Sample.Website.Application;

public interface IProductAppService : Ddd.Application.Abstractions.IApplicationService
{
    Task<List<ProductDto>> GetListAsync();
    Task<ProductDto> UpdatePriceAsync(Guid productId, UpdatePriceInput input);
}

/// <summary>
/// 商品应用服务：列表走 ICaching 缓存；改价是管理操作，
/// 经 IPermissionChecker 校验当前用户（角色）持有 Website.Products.Manage 权限。
/// </summary>
[ApplicationService]
internal sealed class ProductAppService(
    IRepository<Product, Guid> products,
    ICachingProvider cachingProvider,
    IPermissionChecker permissionChecker,
    ICurrentPrincipalAccessor principalAccessor,
    IStringLocalizer<WebsiteResource> localizer) : IProductAppService
{
    private const string CacheName = "website:products";
    private const string ListCacheKey = "all";

    public async Task<List<ProductDto>> GetListAsync()
    {
        var caching = cachingProvider.GetCaching(CacheName);
        var cached = await caching.GetAsync<List<ProductDto>>(ListCacheKey);
        if (cached is not null)
        {
            return cached;
        }

        var list = await products.GetListAsync();
        var dtos = list.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();
        await caching.SetAsync(ListCacheKey, dtos, TimeSpan.FromMinutes(5));
        return dtos;
    }

    public async Task<ProductDto> UpdatePriceAsync(Guid productId, UpdatePriceInput input)
    {
        var granted = await permissionChecker.IsGrantedAsync(principalAccessor.Principal, WebsitePermissions.ManageProducts);
        if (!granted)
        {
            throw new Skywalker.Exceptions.AuthorizationException(localizer["Error:ManageProductsDenied"]);
        }

        var product = await products.GetAsync(productId);
        product.Price = input.Price;
        await products.UpdateAsync(product, autoSave: true);

        cachingProvider.GetCaching(CacheName).Remove(ListCacheKey);
        return new ProductDto(product.Id, product.Name, product.Price, product.Stock);
    }
}

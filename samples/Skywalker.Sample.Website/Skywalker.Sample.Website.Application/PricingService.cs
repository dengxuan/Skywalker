using Skywalker.DependencyInjection;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.Sample.Website.Application;

/// <summary>
/// 定价服务：接口继承 IInterceptable，DynamicProxy SG 在编译期生成静态代理，
/// AddInterceptedServices() 后所有调用经过 AuditInterceptor（零运行时 IL Emit）。
/// </summary>
public interface IPricingService : IInterceptable
{
    Task<decimal> CalculatePayableAsync(decimal totalAmount, decimal freeShippingThreshold);
}

[Service]
internal sealed class PricingService : IPricingService
{
    public Task<decimal> CalculatePayableAsync(decimal totalAmount, decimal freeShippingThreshold)
    {
        // 满额免运费，否则加 10 元运费
        var payable = totalAmount >= freeShippingThreshold ? totalAmount : totalAmount + 10m;
        return Task.FromResult(payable);
    }
}

using Skywalker.Ddd.Domain.Repositories;
using Skywalker.DependencyInjection;
using Skywalker.Sample.Website.Domain;
using Skywalker.Settings.Abstractions;
using Skywalker.Validation;

namespace Skywalker.Sample.Website.Application;

public interface IOrderAppService : Ddd.Application.Abstractions.IApplicationService
{
    Task<OrderDto> PlaceAsync(PlaceOrderInput input);
    Task<OrderDto> ConfirmAsync(Guid orderId);
    Task<OrderDto?> FindAsync(Guid orderId);
}

/// <summary>
/// 订单应用服务：FluentValidation 校验输入 → 聚合根封装业务不变量 →
/// ISettingProvider 读免邮门槛 → IPricingService（静态代理拦截）算应付 →
/// 确认后领域事件经事件总线发确认邮件。
/// </summary>
[ApplicationService]
internal sealed class OrderAppService(
    IRepository<Order, Guid> orders,
    IRepository<Product, Guid> products,
    IValidator<PlaceOrderInput> placeValidator,
    ISettingProvider settings,
    IPricingService pricing) : IOrderAppService
{
    public async Task<OrderDto> PlaceAsync(PlaceOrderInput input)
    {
        var validation = await placeValidator.ValidateAsync(input);
        if (!validation.IsValid)
        {
            throw new Skywalker.Exceptions.SkywalkerValidationException(
                string.Join("; ", validation.Errors.Select(error => error.ErrorMessage)));
        }

        var order = new Order(Guid.NewGuid(), $"SO-{Guid.NewGuid():N}"[..20].ToUpperInvariant(), input.CustomerEmail);

        foreach (var item in input.Items)
        {
            var product = await products.GetAsync(item.ProductId);
            order.AddItem(product, item.Quantity);
        }

        await orders.InsertAsync(order, autoSave: true);
        return await ToDtoAsync(order);
    }

    public async Task<OrderDto> ConfirmAsync(Guid orderId)
    {
        var order = await orders.GetAsync(orderId);
        order.Confirm();
        await orders.UpdateAsync(order, autoSave: true);
        return await ToDtoAsync(order);
    }

    public async Task<OrderDto?> FindAsync(Guid orderId)
    {
        var order = await orders.FindAsync(orderId);
        return order is null ? null : await ToDtoAsync(order);
    }

    private async Task<OrderDto> ToDtoAsync(Order order)
    {
        var threshold = decimal.Parse(await settings.GetOrNullAsync(WebsiteSettings.FreeShippingThreshold) ?? "0");
        var payable = await pricing.CalculatePayableAsync(order.TotalAmount, threshold);

        return new OrderDto(
            order.Id,
            order.OrderNo,
            order.CustomerEmail,
            order.TotalAmount,
            payable,
            order.Status.ToString(),
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList());
    }
}

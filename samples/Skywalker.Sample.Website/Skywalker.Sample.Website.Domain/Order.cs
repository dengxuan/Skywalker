using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Sample.Website.Domain;

/// <summary>
/// 订单聚合根：演示聚合封装、业务不变量与领域事件（AddDistributedEvent → EventBus）。
/// </summary>
public class Order : AggregateRoot<Guid>
{
    public string OrderNo { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    protected Order() { }

    public Order(Guid id, string orderNo, string customerEmail) : base(id)
    {
        OrderNo = orderNo;
        CustomerEmail = customerEmail;
        Status = OrderStatus.Pending;
    }

    public void AddItem(Product product, int quantity)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("只有待处理订单可以添加商品");
        }

        Items.Add(new OrderItem(Guid.NewGuid(), Id, product.Id, product.Name, product.Price, quantity));
        TotalAmount = Items.Sum(item => item.UnitPrice * item.Quantity);
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("只有待处理订单可以确认");
        }

        if (Items.Count == 0)
        {
            throw new InvalidOperationException("订单必须包含至少一个商品");
        }

        Status = OrderStatus.Confirmed;
        AddDistributedEvent(new OrderConfirmedEvent(Id, OrderNo, CustomerEmail, TotalAmount));
    }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Cancelled,
}

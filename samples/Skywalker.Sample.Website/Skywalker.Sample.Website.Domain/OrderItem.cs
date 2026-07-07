using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Sample.Website.Domain;

public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    protected OrderItem() { }

    public OrderItem(Guid id, Guid orderId, Guid productId, string productName, decimal unitPrice, int quantity) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}

// ECommerce.Domain/Entities/OrderItem.cs
namespace Livy.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Subtotal => UnitPrice * Quantity;  // calculado

    private OrderItem() { }
    public OrderItem(Guid orderId, Guid productId, decimal unitPrice, int quantity)
    {
        OrderId = orderId; ProductId = productId;
        UnitPrice = unitPrice; Quantity = quantity;
    }
}

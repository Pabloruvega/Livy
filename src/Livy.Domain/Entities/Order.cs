// Livy.Domain/Entities/Order.cs
using Livy.Domain.Enums;

namespace Livy.Domain.Entities;

public class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }

    // Colección de items — EF Core la gestiona como relación 1:N
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }  // para EF Core

    public Order(Guid userId)
    {
        UserId    = userId;
        CreatedAt = DateTime.UtcNow;
        Status    = OrderStatus.Pending;
        Total     = 0;
    }

    public void AddItem(Product product, int quantity)
    {
        product.ReduceStock(quantity);  // regla de negocio en el dominio
        var item = new OrderItem(Id, product.Id, product.Price, quantity);
        _items.Add(item);
        Total += item.Subtotal;
    }
}

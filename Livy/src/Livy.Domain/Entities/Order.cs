// Livy.Domain/Entities/Order.cs
using Livy.Domain.Enums;
using Livy.Domain.Exceptions;

namespace Livy.Domain.Entities;

public class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }
    public string? PaymentTransactionCode { get; private set; }

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
        if (!product.IsActive)
            throw new DomainException($"El producto '{product.Name}' no está disponible.");

        product.ReduceStock(quantity);  // regla de negocio en el dominio
        var item = new OrderItem(Id, product.Id, product.Price, quantity);
        _items.Add(item);
        Total += item.Subtotal;
    }

    // El pago se resuelve como un paso posterior a la creación (llamada al PaymentService).
    // Solo una orden Pending puede pasar a Paid o PaymentRejected.
    public void MarkAsPaid(string transactionCode)
    {
        EnsurePending("marcar como pagada");
        Status = OrderStatus.Paid;
        PaymentTransactionCode = transactionCode;
    }

    public void RejectPayment(string transactionCode)
    {
        EnsurePending("rechazar el pago de");
        Status = OrderStatus.PaymentRejected;
        PaymentTransactionCode = transactionCode;
    }

    // Se llama ANTES de contactar al PaymentService, así una orden que ya no está
    // Pending falla aunque el servicio de pagos no responda.
    public void EnsureCanProcessPayment() => EnsurePending("procesar el pago de");

    public bool IsOwnedBy(Guid userId) => UserId == userId;

    private void EnsurePending(string action)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException(
                $"No se puede {action} la orden {Id}: su estado es {Status} y debe ser {OrderStatus.Pending}.");
    }
}

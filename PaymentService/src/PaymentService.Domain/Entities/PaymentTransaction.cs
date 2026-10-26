using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Entities;

public class PaymentTransaction : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string TransactionCode { get; private set; } = string.Empty;
    public DateTime ProcessedAt { get; private set; }

    private PaymentTransaction() { } // EF Core

    // Constructor de negocio — la regla de aprobación vive acá, no en el Handler.
    private PaymentTransaction(Guid orderId, Money amount, decimal approvalLimit)
    {
        OrderId = orderId;
        Amount = amount;
        ProcessedAt = DateTime.UtcNow;
        TransactionCode = $"TX-{Id.ToString()[..8].ToUpperInvariant()}";
        Status = amount.Value < approvalLimit ? PaymentStatus.Approved : PaymentStatus.Rejected;
    }

    public static PaymentTransaction Process(Guid orderId, decimal amount, decimal approvalLimit)
        => new(orderId, new Money(amount), approvalLimit);
}

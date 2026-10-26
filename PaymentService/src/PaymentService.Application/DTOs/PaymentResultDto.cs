using PaymentService.Domain.Entities;

namespace PaymentService.Application.DTOs;

public record PaymentResultDto(
    Guid TransactionId,
    Guid OrderId,
    string Status,
    string TransactionCode,
    decimal Amount,
    DateTime ProcessedAt)
{
    public static PaymentResultDto FromEntity(PaymentTransaction t) => new(
        t.Id, t.OrderId, t.Status.ToString(), t.TransactionCode, t.Amount.Value, t.ProcessedAt);
}

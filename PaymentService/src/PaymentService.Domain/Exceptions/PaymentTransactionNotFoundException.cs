namespace PaymentService.Domain.Exceptions;

public class PaymentTransactionNotFoundException : NotFoundException
{
    public PaymentTransactionNotFoundException(Guid orderId)
        : base($"No existe una transacción de pago para la orden '{orderId}'.") { }
}

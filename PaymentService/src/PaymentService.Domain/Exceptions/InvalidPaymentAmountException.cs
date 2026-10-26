namespace PaymentService.Domain.Exceptions;

public class InvalidPaymentAmountException : DomainException
{
    public InvalidPaymentAmountException(decimal amount)
        : base($"El monto '{amount}' no es válido: debe ser mayor a cero.") { }
}

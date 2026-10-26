using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.ValueObjects;

// Value Object: dos Money son iguales si su Value es igual (record struct da esa
// igualdad estructural gratis). Inmutable y se autovalida al construirse.
public readonly record struct Money
{
    public decimal Value { get; }

    public Money(decimal value)
    {
        if (value <= 0)
            throw new InvalidPaymentAmountException(value);

        Value = value;
    }

    public static implicit operator decimal(Money money) => money.Value;

    public override string ToString() => Value.ToString("C");
}

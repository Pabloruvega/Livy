// ValueObjects/Money.cs 

using Livy.Domain.Exceptions;
namespace Livy.Domain.ValueObjects; 
 
public sealed class Money : ValueObject 
{ 
    public decimal Amount { get; } 
    public string Currency { get; } 
 
    private Money(decimal amount, string currency) 
    { 
        Amount = amount; 
        Currency = currency; 
    } 
 
    public static Money Create(decimal amount, string currency = "ARS") 
    { 
        if (amount < 0) throw new DomainException("El monto no puede ser negativo."); 
        if (string.IsNullOrEmpty(currency)) throw new DomainException("La moneda es requerida."); 
        return new Money(amount, currency.ToUpper()); 
    } 
 
    public Money Add(Money other) 
    { 
        if (Currency != other.Currency) throw new DomainException("No se pueden sumar monedas diferentes."); 

return new Money(Amount + other.Amount, Currency); 
} 
protected override IEnumerable<object> GetEqualityComponents() 
{ 
yield return Amount; 
yield return Currency; 
} 
public override string ToString() => $"{Amount} {Currency}"; 
}
// ValueObjects/Email.cs 
using Livy.Domain.Exceptions; 
 
namespace Livy.Domain.ValueObjects; 
 
public sealed class Email : ValueObject 
{ 
    public string Value { get; } 
 
    private Email(string value) => Value = value; 
 
    public static Email Create(string email) 

    { 
        if (string.IsNullOrWhiteSpace(email)) 
            throw new DomainException("El email es obligatorio."); 
 
        if (!email.Contains('@')) 
            throw new DomainException("El formato del email es inválido."); 
 
        return new Email(email.Trim().ToLower()); 
    } 
 
    protected override IEnumerable<object> GetEqualityComponents() 
    { 
        yield return Value; 
    } 
} 
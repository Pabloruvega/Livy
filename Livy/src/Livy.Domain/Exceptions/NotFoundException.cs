// Exceptions/NotFoundException.cs
namespace Livy.Domain.Exceptions;

public abstract class NotFoundException : DomainException
{
    protected NotFoundException(string message) : base(message) { }
}

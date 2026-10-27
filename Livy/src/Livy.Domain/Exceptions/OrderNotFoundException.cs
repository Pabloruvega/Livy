// Exceptions/OrderNotFoundException.cs
namespace Livy.Domain.Exceptions;

public class OrderNotFoundException : NotFoundException
{
    public OrderNotFoundException(Guid id)
        : base($"Orden con Id '{id}' no encontrada.") { }
}

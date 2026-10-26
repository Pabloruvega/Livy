// Exceptions/UserNotFoundException.cs
namespace Livy.Domain.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid id)
        : base($"Usuario con Id '{id}' no encontrado.") { }
}

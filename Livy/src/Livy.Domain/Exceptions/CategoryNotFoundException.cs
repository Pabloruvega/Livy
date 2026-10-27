// Exceptions/CategoryNotFoundException.cs
namespace Livy.Domain.Exceptions;

public class CategoryNotFoundException : NotFoundException
{
    public CategoryNotFoundException(Guid id)
        : base($"Categoría con Id '{id}' no encontrada.") { }
}

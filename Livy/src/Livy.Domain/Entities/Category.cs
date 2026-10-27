using Livy.Domain.Exceptions;

namespace Livy.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
   

    // Relación con Productos (opcional, según tu lógica)
    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { } // Para EF Core

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la categoría es obligatorio.");

        Name = name;
    }

    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la categoría es obligatorio.");

        Name = name;
    }
}
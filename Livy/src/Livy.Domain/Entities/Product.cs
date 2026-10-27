// Livy.Domain/Entities/Product.cs
using Livy.Domain.Exceptions;

namespace Livy.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public Guid CategoryId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    // Constructor privado para EF Core
    private Product() { }

    // Constructor de negocio — valida las reglas del dominio
    public Product(string name, string description, decimal price, int stock, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre es obligatorio.");
        if (price < 0) throw new DomainException("El precio no puede ser negativo.");
        if (stock < 0) throw new DomainException("El stock no puede ser negativo.");

        Name        = name;
        Description = description;
        Price       = price;
        Stock       = stock;
        CategoryId  = categoryId;
        CreatedAt   = DateTime.UtcNow;
        IsActive    = true;
    }

    // Métodos de negocio
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new DomainException("Precio inválido.");
        Price = newPrice;
    }

    public void Update(string name, string description, decimal price, int stock, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre es obligatorio.");
        if (price < 0) throw new DomainException("El precio no puede ser negativo.");
        if (stock < 0) throw new DomainException("El stock no puede ser negativo.");

        Name        = name;
        Description = description;
        Price       = price;
        Stock       = stock;
        CategoryId  = categoryId;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0) throw new DomainException("La cantidad debe ser mayor a cero.");
        if (quantity > Stock) throw new InsufficientStockException(Name, quantity, Stock);
        Stock -= quantity;
    }

    // Compensación cuando un pago es rechazado: el stock reservado vuelve a estar disponible.
    public void RestoreStock(int quantity)
    {
        if (quantity <= 0) throw new DomainException("La cantidad debe ser mayor a cero.");
        Stock += quantity;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}

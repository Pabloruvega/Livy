// Livy.Domain/Entities/Product.cs
namespace Livy.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public Guid CategoryId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Constructor privado para EF Core
    private Product() { }

    // Constructor de negocio — valida las reglas del dominio
    public Product(string name, string description, decimal price, int stock, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre es obligatorio.");
        if (price < 0) throw new ArgumentException("El precio no puede ser negativo.");
        if (stock < 0) throw new ArgumentException("El stock no puede ser negativo.");

        Name        = name;
        Description = description;
        Price       = price;
        Stock       = stock;
        CategoryId  = categoryId;
        CreatedAt   = DateTime.UtcNow;
    }

    // Métodos de negocio
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Precio inválido.");
        Price = newPrice;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity > Stock) throw new InvalidOperationException("Stock insuficiente.");
        Stock -= quantity;
    }
}

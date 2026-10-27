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
        Name = name;
    
        // CreatedAt ya viene de BaseEntity
    }

    public void Update(string name)
    {
        Name = name;
       
    }
}
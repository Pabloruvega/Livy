// Application/DTOs/Product/ProductDto.cs
namespace Livy.Application.DTOs.Product;

/// <summary>Response: datos de un producto hacia el cliente</summary>
public class ProductDto
{
    public Guid     Id          { get; set; }
    public string   Name        { get; set; } = string.Empty;
    public string   Description { get; set; } = string.Empty;
    public decimal  Price       { get; set; }
    public int      Stock       { get; set; }
    public Guid     CategoryId  { get; set; }
    public DateTime CreatedAt   { get; set; }
    public bool     IsActive    { get; set; }
}
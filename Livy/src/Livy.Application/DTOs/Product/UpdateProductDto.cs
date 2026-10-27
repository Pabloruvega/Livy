// Application/DTOs/Product/UpdateProductDto.cs
namespace Livy.Application.DTOs.Product;

/// <summary>Request: datos para actualizar un producto existente. Validado por UpdateProductCommandValidator (FluentValidation).</summary>
public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
}
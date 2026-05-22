// Application/DTOs/Product/UpdateProductDto.cs
using System.ComponentModel.DataAnnotations;

namespace Livy.Application.DTOs.Product;

/// <summary>Request: datos para actualizar un producto existente</summary>
public class UpdateProductDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}
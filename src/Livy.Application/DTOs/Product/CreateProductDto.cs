// Application/DTOs/Product/CreateProductDto.cs
using System.ComponentModel.DataAnnotations;

namespace Livy.Application.DTOs.Product;

/// <summary>Request: datos para crear un producto</summary>
public class CreateProductDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public Guid CategoryId { get; set; }
}
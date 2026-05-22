// Application/DTOs/Category/CreateCategoryDto.cs
using System.ComponentModel.DataAnnotations;

namespace Livy.Application.DTOs.Category;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
// Application/DTOs/Category/CreateCategoryDto.cs
namespace Livy.Application.DTOs.Category;

// La validación vive en CreateCategoryCommandValidator (FluentValidation).
public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
}

// Application/DTOs/Category/CategoryDto.cs
namespace Livy.Application.DTOs.Category;

public class CategoryDto
{
    public Guid   Id   { get; set; }
    public string Name { get; set; } = string.Empty;
}
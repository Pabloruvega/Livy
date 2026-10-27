// Application/Features/Categories/CategoryMappings.cs
using Livy.Application.DTOs.Category;
using Livy.Domain.Entities;

namespace Livy.Application.Features.Categories;

// Único mapeo Category → CategoryDto, compartido por los handlers de Categories.
public static class CategoryMappings
{
    public static CategoryDto ToDto(this Category category) => new()
    {
        Id   = category.Id,
        Name = category.Name
    };
}

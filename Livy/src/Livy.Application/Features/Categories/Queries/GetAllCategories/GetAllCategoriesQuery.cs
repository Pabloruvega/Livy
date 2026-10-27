// Application/Features/Categories/Queries/GetAllCategories/GetAllCategoriesQuery.cs
using Livy.Application.DTOs.Category;
using MediatR;

namespace Livy.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery() : IRequest<IEnumerable<CategoryDto>>;

// Application/Features/Categories/Queries/GetCategoryById/GetCategoryByIdQuery.cs
using Livy.Application.DTOs.Category;
using MediatR;

namespace Livy.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid CategoryId) : IRequest<CategoryDto>;

// Application/Features/Categories/Commands/CreateCategory/CreateCategoryCommand.cs
using Livy.Application.DTOs.Category;
using MediatR;

namespace Livy.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name) : IRequest<CategoryDto>;

// Application/Features/Categories/Commands/DeleteCategory/DeleteCategoryCommand.cs
using MediatR;

namespace Livy.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid CategoryId) : IRequest<bool>;

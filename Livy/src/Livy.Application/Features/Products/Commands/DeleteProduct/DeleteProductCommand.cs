// Application/Features/Products/Commands/DeleteProduct/DeleteProductCommand.cs
using MediatR;

namespace Livy.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid ProductId) : IRequest<bool>;
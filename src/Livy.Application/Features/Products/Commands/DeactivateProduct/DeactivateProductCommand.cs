// Application/Features/Products/Commands/DeactivateProduct/DeactivateProductCommand.cs
using Livy.Application.DTOs.Product;
using MediatR;

namespace Livy.Application.Features.Products.Commands.DeactivateProduct;

public record DeactivateProductCommand(Guid ProductId) : IRequest<ProductDto>;

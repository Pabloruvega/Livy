// Application/Features/Products/Commands/ActivateProduct/ActivateProductCommand.cs
using Livy.Application.DTOs.Product;
using MediatR;

namespace Livy.Application.Features.Products.Commands.ActivateProduct;

public record ActivateProductCommand(Guid ProductId) : IRequest<ProductDto>;

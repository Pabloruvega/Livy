// Application/Features/Products/Commands/UpdateProduct/UpdateProductCommand.cs
using Livy.Application.DTOs.Product;
using MediatR;

namespace Livy.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid    ProductId,
    string  Name,
    string  Description,
    decimal Price,
    int     Stock,
    Guid    CategoryId
) : IRequest<ProductDto>;
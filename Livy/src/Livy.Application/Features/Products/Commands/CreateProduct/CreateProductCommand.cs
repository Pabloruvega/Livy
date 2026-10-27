// Application/Features/Products/Commands/CreateProduct/CreateProductCommand.cs
using Livy.Application.DTOs.Product;
using MediatR;

namespace Livy.Application.Features.Products.Commands.CreateProduct;


public record CreateProductCommand(
    string  Name,
    string  Description,
    decimal Price,
    int     Stock,
    Guid    CategoryId
) : IRequest<ProductDto>;
// Application/Features/Products/Queries/GetProductById/GetProductByIdQuery.cs
using Livy.Application.DTOs.Product;
using MediatR;

namespace Livy.Application.Features.Products.Queries.GetProductById;

// IsAdmin: el Admin ve también los productos inactivos; para el resto, un inactivo no existe (404).
public record GetProductByIdQuery(Guid ProductId, bool IsAdmin) : IRequest<ProductDto>;
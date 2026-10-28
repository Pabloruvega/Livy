// Application/Features/Products/Queries/GetAllProducts/GetAllProductsQuery.cs
using Livy.Application.DTOs.Product;
using MediatR;

namespace Livy.Application.Features.Products.Queries.GetAllProducts;


// IsAdmin: el Admin ve también los productos inactivos; el resto, solo el catálogo activo.
public record GetAllProductsQuery(bool IsAdmin) : IRequest<IEnumerable<ProductDto>>;
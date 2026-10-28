// Application/Features/Products/Queries/SearchProducts/SearchProductsQuery.cs
using Livy.Application.DTOs.Product;
using MediatR;

namespace Livy.Application.Features.Products.Queries.SearchProducts;

// IsAdmin: el Admin ve también los productos inactivos; el resto, solo el catálogo activo.
public record SearchProductsQuery(string Term, bool IsAdmin) : IRequest<IEnumerable<ProductDto>>;

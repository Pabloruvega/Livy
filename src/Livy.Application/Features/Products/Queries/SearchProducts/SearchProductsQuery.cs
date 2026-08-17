// Application/Features/Products/Queries/SearchProducts/SearchProductsQuery.cs
using Livy.Application.DTOs.Product;
using MediatR;

namespace Livy.Application.Features.Products.Queries.SearchProducts;

public record SearchProductsQuery(string Term) : IRequest<IEnumerable<ProductDto>>;

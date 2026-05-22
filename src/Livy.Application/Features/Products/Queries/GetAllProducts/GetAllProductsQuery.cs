// Application/Features/Products/Queries/GetAllProducts/GetAllProductsQuery.cs
using Livy.Application.DTOs.Product;
using MediatR;

namespace Livy.Application.Features.Products.Queries.GetAllProducts;


public record GetAllProductsQuery() : IRequest<IEnumerable<ProductDto>>;
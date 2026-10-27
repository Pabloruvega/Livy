// Application/Features/Products/Queries/SearchProducts/SearchProductsHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Product;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsHandler
    : IRequestHandler<SearchProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<SearchProductsHandler> _logger;

    public SearchProductsHandler(IProductRepository repo, ILogger<SearchProductsHandler> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> Handle(
        SearchProductsQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando SearchProductsQuery con término '{Term}'.", request.Term);
        var products = await _repo.SearchByNameAsync(request.Term, ct);

        return products.Select(p => new ProductDto
        {
            Id          = p.Id,
            Name        = p.Name,
            Description = p.Description,
            Price       = p.Price,
            Stock       = p.Stock,
            CategoryId  = p.CategoryId,
            CreatedAt   = p.CreatedAt,
            IsActive    = p.IsActive
        });
    }
}

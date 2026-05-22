// Application/UseCases/Products/Queries/GetAllProducts/GetAllProductsHandler.cs
using Livy.Application.Common;
using Livy.Application.DTOs.Product;
using Livy.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Livy.Application.UseCases.Products.Queries.GetAllProducts;

public class GetAllProductsHandler : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetAllProductsHandler> _logger;

    public GetAllProductsHandler(IProductRepository repo, ILogger<GetAllProductsHandler> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> HandleAsync(
        GetAllProductsQuery query, CancellationToken ct = default)
    {
        _logger.LogInformation("Ejecutando GetAllProductsQuery.");
        var products = await _repo.GetAllAsync(ct);

        return products.Select(p => new ProductDto
        {
            Id          = p.Id,
            Name        = p.Name,
            Description = p.Description,
            Price       = p.Price,
            Stock       = p.Stock,
            CategoryId  = p.CategoryId,
            CreatedAt   = p.CreatedAt
        });
    }
}
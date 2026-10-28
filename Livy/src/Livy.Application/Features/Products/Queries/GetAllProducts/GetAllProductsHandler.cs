// Application/Features/Products/Queries/GetAllProducts/GetAllProductsHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Product;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsHandler
    : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetAllProductsHandler> _logger;

    public GetAllProductsHandler(IProductRepository repo, ILogger<GetAllProductsHandler> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> Handle(
        GetAllProductsQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando GetAllProductsQuery.");
        var products = request.IsAdmin
            ? await _repo.GetAllAsync(ct)
            : await _repo.GetActiveProductsAsync(ct);

        return products.Select(p => p.ToDto());
    }
}
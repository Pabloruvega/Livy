// Application/UseCases/Products/Queries/GetProductById/GetProductByIdHandler.cs
using Livy.Application.Common;
using Livy.Application.DTOs.Product;
using Livy.Application.Interfaces;
using Livy.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Livy.Application.UseCases.Products.Queries.GetProductById;

public class GetProductByIdHandler : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductByIdHandler> _logger;

    public GetProductByIdHandler(IProductRepository repo, ILogger<GetProductByIdHandler> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<ProductDto> HandleAsync(
        GetProductByIdQuery query, CancellationToken ct = default)
    {
        _logger.LogInformation("Ejecutando GetProductByIdQuery para Id {Id}.", query.ProductId);

        var product = await _repo.GetByIdAsync(query.ProductId, ct)
            ?? throw new ProductNotFoundException(query.ProductId);

        return new ProductDto
        {
            Id          = product.Id,
            Name        = product.Name,
            Description = product.Description,
            Price       = product.Price,
            Stock       = product.Stock,
            CategoryId  = product.CategoryId,
            CreatedAt   = product.CreatedAt
        };
    }
}
// Application/Features/Products/Queries/GetProductById/GetProductByIdHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Product;
using Livy.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductByIdHandler> _logger;

    public GetProductByIdHandler(IProductRepository repo, ILogger<GetProductByIdHandler> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando GetProductByIdQuery para Id {Id}.", request.ProductId);

        var product = await _repo.GetByIdAsync(request.ProductId, ct)
            ?? throw new ProductNotFoundException(request.ProductId);

        // Mismo criterio que el catálogo: fuera del Admin, un producto inactivo no se expone.
        if (!product.IsActive && !request.IsAdmin)
            throw new ProductNotFoundException(request.ProductId);

        return product.ToDto();
    }
}
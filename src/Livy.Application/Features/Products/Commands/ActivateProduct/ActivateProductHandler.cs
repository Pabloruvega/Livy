// Application/Features/Products/Commands/ActivateProduct/ActivateProductHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Product;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Products.Commands.ActivateProduct;

public class ActivateProductHandler : IRequestHandler<ActivateProductCommand, ProductDto>
{
    private readonly IProductRepository _repo;
    private readonly IUnitOfWork        _unitOfWork;
    private readonly ILogger<ActivateProductHandler> _logger;

    public ActivateProductHandler(
        IProductRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<ActivateProductHandler> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<ProductDto> Handle(ActivateProductCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando ActivateProductCommand para Id {Id}.", request.ProductId);

        var product = await _repo.GetByIdAsync(request.ProductId, ct)
            ?? throw new ProductNotFoundException(request.ProductId);

        product.Activate();

        await _repo.UpdateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Producto {Id} activado.", request.ProductId);

        return new ProductDto
        {
            Id          = product.Id,
            Name        = product.Name,
            Description = product.Description,
            Price       = product.Price,
            Stock       = product.Stock,
            CategoryId  = product.CategoryId,
            CreatedAt   = product.CreatedAt,
            IsActive    = product.IsActive
        };
    }
}

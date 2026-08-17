// Application/Features/Products/Commands/DeactivateProduct/DeactivateProductHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Product;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Products.Commands.DeactivateProduct;

public class DeactivateProductHandler : IRequestHandler<DeactivateProductCommand, ProductDto>
{
    private readonly IProductRepository _repo;
    private readonly IUnitOfWork        _unitOfWork;
    private readonly ILogger<DeactivateProductHandler> _logger;

    public DeactivateProductHandler(
        IProductRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateProductHandler> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<ProductDto> Handle(DeactivateProductCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando DeactivateProductCommand para Id {Id}.", request.ProductId);

        var product = await _repo.GetByIdAsync(request.ProductId, ct)
            ?? throw new ProductNotFoundException(request.ProductId);

        product.Deactivate();

        await _repo.UpdateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Producto {Id} desactivado.", request.ProductId);

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

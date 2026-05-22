// Application/Features/Products/Commands/UpdateProduct/UpdateProductHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Product;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _repo;
    private readonly IUnitOfWork        _unitOfWork;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(
        IProductRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<UpdateProductHandler> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando UpdateProductCommand para Id {Id}.", request.ProductId);

        var product = await _repo.GetByIdAsync(request.ProductId, ct)
            ?? throw new ProductNotFoundException(request.ProductId);

        product.UpdatePrice(request.Price);

        await _repo.UpdateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Producto {Id} actualizado.", request.ProductId);

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
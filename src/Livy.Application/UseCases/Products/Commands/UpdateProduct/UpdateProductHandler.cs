// Application/UseCases/Products/Commands/UpdateProduct/UpdateProductHandler.cs
using Livy.Application.Common;
using Livy.Application.DTOs.Product;
using Livy.Application.Interfaces;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Livy.Application.UseCases.Products.Commands.UpdateProduct;

public class UpdateProductHandler : ICommandHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
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

    public async Task<ProductDto> HandleAsync(
        UpdateProductCommand command, CancellationToken ct = default)
    {
        _logger.LogInformation("Ejecutando UpdateProductCommand para Id {Id}.", command.ProductId);

        var product = await _repo.GetByIdAsync(command.ProductId, ct)
            ?? throw new ProductNotFoundException(command.ProductId);

        // Los métodos del dominio validan las reglas de negocio
        product.UpdatePrice(command.Price);

        await _repo.UpdateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Producto {Id} actualizado.", command.ProductId);

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
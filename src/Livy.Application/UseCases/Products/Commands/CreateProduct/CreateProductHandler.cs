// Application/UseCases/Products/Commands/CreateProduct/CreateProductHandler.cs
using Livy.Application.Common;
using Livy.Application.DTOs.Product;
using Livy.Application.Interfaces;
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Livy.Application.UseCases.Products.Commands.CreateProduct;

public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        IProductRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<CreateProductHandler> logger)
    {
        _repo        = repo;
        _unitOfWork  = unitOfWork;
        _logger      = logger;
    }

    public async Task<ProductDto> HandleAsync(
        CreateProductCommand command, CancellationToken ct = default)
    {
        _logger.LogInformation("Ejecutando CreateProductCommand para '{Name}'.", command.Name);

        var product = new Product(
            command.Name,
            command.Description,
            command.Price,
            command.Stock,
            command.CategoryId);

        await _repo.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct); // IUnitOfWork persiste todo de una sola vez

        _logger.LogInformation("Producto creado con Id {Id}.", product.Id);

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
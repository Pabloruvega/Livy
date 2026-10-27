// Application/Features/Products/Commands/CreateProduct/CreateProductHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Product;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Products.Commands.CreateProduct;

public class CreateProductHandler
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository  _repo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        IProductRepository repo,
        ICategoryRepository categoryRepo,
        IUnitOfWork unitOfWork,
        ILogger<CreateProductHandler> logger)
    {
        _repo         = repo;
        _categoryRepo = categoryRepo;
        _unitOfWork   = unitOfWork;
        _logger       = logger;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando CreateProductCommand para '{Name}'.", request.Name);

        // Sin esto, una CategoryId inexistente termina en un 500 por violación de FK al guardar
        if (!await _categoryRepo.ExistsAsync(request.CategoryId, ct))
            throw new CategoryNotFoundException(request.CategoryId);

        // La entidad valida las reglas de negocio en su constructor
        var product = new Product(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.CategoryId);

        await _repo.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Producto creado con Id {Id}.", product.Id);

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
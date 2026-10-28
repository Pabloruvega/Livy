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
    private readonly IProductRepository  _repo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(
        IProductRepository repo,
        ICategoryRepository categoryRepo,
        IUnitOfWork unitOfWork,
        ILogger<UpdateProductHandler> logger)
    {
        _repo         = repo;
        _categoryRepo = categoryRepo;
        _unitOfWork   = unitOfWork;
        _logger       = logger;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando UpdateProductCommand para Id {Id}.", request.ProductId);

        var product = await _repo.GetByIdAsync(request.ProductId, ct)
            ?? throw new ProductNotFoundException(request.ProductId);

        // Sin esto, una CategoryId inexistente termina en un 500 por violación de FK al guardar
        if (!await _categoryRepo.ExistsAsync(request.CategoryId, ct))
            throw new CategoryNotFoundException(request.CategoryId);

        product.Update(request.Name, request.Description, request.Price, request.Stock, request.CategoryId);

        await _repo.UpdateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Producto {Id} actualizado.", request.ProductId);

        return product.ToDto();
    }
}
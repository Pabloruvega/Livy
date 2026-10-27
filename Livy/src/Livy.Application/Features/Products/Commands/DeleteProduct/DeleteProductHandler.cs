// Application/Features/Products/Commands/DeleteProduct/DeleteProductHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repo;
    private readonly IUnitOfWork        _unitOfWork;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(
        IProductRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<DeleteProductHandler> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando DeleteProductCommand para Id {Id}.", request.ProductId);

        var product = await _repo.GetByIdAsync(request.ProductId, ct)
            ?? throw new ProductNotFoundException(request.ProductId);

        await _repo.DeleteAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Producto {Id} eliminado.", request.ProductId);
        return true;
    }
}
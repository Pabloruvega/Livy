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
    private readonly IOrderRepository   _orderRepo;
    private readonly IUnitOfWork        _unitOfWork;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(
        IProductRepository repo,
        IOrderRepository orderRepo,
        IUnitOfWork unitOfWork,
        ILogger<DeleteProductHandler> logger)
    {
        _repo       = repo;
        _orderRepo  = orderRepo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando DeleteProductCommand para Id {Id}.", request.ProductId);

        var product = await _repo.GetByIdAsync(request.ProductId, ct)
            ?? throw new ProductNotFoundException(request.ProductId);

        // Borrado físico: si ya se vendió, sus OrderItems quedarían apuntando a un producto inexistente.
        if (await _orderRepo.HasOrdersForProductAsync(product.Id, ct))
            throw new DomainException("El producto tiene órdenes asociadas; desactivalo en lugar de eliminarlo.");

        await _repo.DeleteAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Producto {Id} eliminado.", request.ProductId);
        return true;
    }
}
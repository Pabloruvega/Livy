// Application/UseCases/Products/Commands/DeleteProduct/DeleteProductHandler.cs
using Livy.Application.Common;
using Livy.Application.Interfaces;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Livy.Application.UseCases.Products.Commands.DeleteProduct;

public class DeleteProductHandler : ICommandHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
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

    public async Task<bool> HandleAsync(
        DeleteProductCommand command, CancellationToken ct = default)
    {
        _logger.LogInformation("Ejecutando DeleteProductCommand para Id {Id}.", command.ProductId);

        var product = await _repo.GetByIdAsync(command.ProductId, ct)
            ?? throw new ProductNotFoundException(command.ProductId);

        await _repo.DeleteAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Producto {Id} eliminado.", command.ProductId);
        return true;
    }
}
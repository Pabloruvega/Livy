// Application/Features/Categories/Commands/DeleteCategory/DeleteCategoryHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly ILogger<DeleteCategoryHandler> _logger;

    public DeleteCategoryHandler(
        ICategoryRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCategoryHandler> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        // IRepository<T>.DeleteAsync recibe la entidad, no el Guid
        var category = await _repo.GetByIdAsync(request.CategoryId, ct)
            ?? throw new CategoryNotFoundException(request.CategoryId);

        await _repo.DeleteAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Categoría {Id} eliminada.", request.CategoryId);
        return true;
    }
}

// Application/Features/Categories/Commands/CreateCategory/CreateCategoryHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Category;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly ILogger<CreateCategoryHandler> _logger;

    public CreateCategoryHandler(
        ICategoryRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<CreateCategoryHandler> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var exists = await _repo.ExistsAsync(request.Name, ct);
        if (exists)
            throw new DomainException($"Ya existe una categoría con el nombre '{request.Name}'.");

        _logger.LogInformation("Creando categoría '{Name}'.", request.Name);
        var category = new Category(request.Name);

        await _repo.AddAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return category.ToDto();
    }
}

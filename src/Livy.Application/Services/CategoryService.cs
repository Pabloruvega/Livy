// Application/Services/CategoryService.cs
using Livy.Application.DTOs.Category;
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository repo, IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Obteniendo todas las categorías.");
        var cats = await _repo.GetAllAsync(ct);
        return cats.Select(c => new CategoryDto { Id = c.Id, Name = c.Name });
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cat = await _repo.GetByIdAsync(id, ct);
        if (cat is null) return null;
        return new CategoryDto { Id = cat.Id, Name = cat.Name };
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var exists = await _repo.ExistsAsync(dto.Name, ct);
        if (exists)
            throw new DomainException($"Ya existe una categoría con el nombre '{dto.Name}'.");

        _logger.LogInformation("Creando categoría '{Name}'.", dto.Name);
        var category = new Category(dto.Name);
        await _repo.AddAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return new CategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        // IRepository<T>.DeleteAsync recibe la entidad, no el Guid
        var cat = await _repo.GetByIdAsync(id, ct)
            ?? throw new CategoryNotFoundException(id);

        await _repo.DeleteAsync(cat, ct); // ← entidad, no id
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Categoría {Id} eliminada.", id);
    }
}
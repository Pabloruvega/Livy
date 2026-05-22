// Application/Services/CategoryService.cs
using Livy.Application.DTOs.Category;
using Livy.Application.Interfaces;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository repo, ILogger<CategoryService> logger)
    {
        _repo   = repo;
        _logger = logger;
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
        return new CategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var cat = await _repo.GetByIdAsync(id, ct);
        if (cat is null)
            throw new DomainException($"Categoría con Id '{id}' no encontrada.");
        await _repo.DeleteAsync(id, ct);
        _logger.LogInformation("Categoría {Id} eliminada.", id);
    }
}
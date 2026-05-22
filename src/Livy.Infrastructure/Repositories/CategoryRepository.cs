// Infrastructure/Repositories/CategoryRepository.cs
using Livy.Application.Interfaces;
using Livy.Domain.Entities;
using Livy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livy.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly LivyDbContext _context;

    public CategoryRepository(LivyDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Categories.FindAsync(new object[] { id }, ct);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Categories.AsNoTracking().ToListAsync(ct);
    }

    public async Task AddAsync(Category category, CancellationToken ct = default)
    {
        await _context.Categories.AddAsync(category, ct);
        await _context.SaveChangesAsync(ct); // ← FALTABA
    }

    public async Task UpdateAsync(Category category, CancellationToken ct = default)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(ct); // ← FALTABA
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await GetByIdAsync(id, ct);
        if (category is not null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(ct); // ← FALTABA
        }
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken ct = default)
    {
        return await _context.Categories.AnyAsync(c => c.Name == name, ct);
    }
}
// Infrastructure/Repositories/CategoryRepository.cs
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Entities;
using Livy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livy.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly LivyDbContext _ctx;

    public CategoryRepository(LivyDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken ct = default)
        => await _ctx.Categories.AnyAsync(c => c.Name == name, ct);
}
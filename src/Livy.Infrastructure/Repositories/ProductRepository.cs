// Infrastructure/Repositories/ProductRepository.cs
using Livy.Application.Interfaces;
using Livy.Domain.Entities;
using Livy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livy.Infrastructure.Repositories;


public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly LivyDbContext _ctx;

    public ProductRepository(LivyDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(
        string term, CancellationToken ct = default)
        => await _ctx.Products
               .AsNoTracking()
               .Where(p => p.Name.Contains(term))
               .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Products.AnyAsync(p => p.Id == id, ct);
}
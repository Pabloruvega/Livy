// Infrastructure/Repositories/ProductRepository.cs
using Microsoft.EntityFrameworkCore;
using Livy.Application.Interfaces;
using Livy.Domain.Entities;
using Livy.Infrastructure.Persistence;

namespace Livy.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly LivyDbContext _ctx;
    public ProductRepository(LivyDbContext ctx) => _ctx = ctx;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Products.FindAsync(new object[] { id }, ct);

    
    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Products.AsNoTracking().ToListAsync(ct);

    public async Task<IEnumerable<Product>> SearchByNameAsync(
        string term, CancellationToken ct = default)
        => await _ctx.Products
               .AsNoTracking()
               .Where(p => p.Name.Contains(term))
               .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Products.AnyAsync(p => p.Id == id, ct);

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await _ctx.Products.AddAsync(product, ct);
        await _ctx.SaveChangesAsync(ct);  // persiste en BD
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _ctx.Products.Update(product);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _ctx.Products.FindAsync(new object[] { id }, ct);
        if (product is not null)
        {
            _ctx.Products.Remove(product);
            await _ctx.SaveChangesAsync(ct);
        }
    }
}

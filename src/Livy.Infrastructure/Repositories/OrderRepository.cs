// Infrastructure/Repositories/OrderRepository.cs
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Entities;
using Livy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livy.Infrastructure.Repositories;


public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly LivyDbContext _ctx;

    public OrderRepository(LivyDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }

    /// <summary>Trae la orden con sus ítems en un solo JOIN (eager loading).</summary>
    public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Orders
               .Include(o => o.Items)
               .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IEnumerable<Order>> GetByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => await _ctx.Orders
               .AsNoTracking()
               .Where(o => o.UserId == userId)
               .Include(o => o.Items)
               .OrderByDescending(o => o.CreatedAt)
               .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Orders.AnyAsync(o => o.Id == id, ct);
}
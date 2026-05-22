// Infrastructure/Repositories/OrderRepository.cs
using Livy.Application.Interfaces;
using Livy.Domain.Entities;
using Livy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livy.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly LivyDbContext _context;

    public OrderRepository(LivyDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await _context.Orders.AddAsync(order, ct);
        await _context.SaveChangesAsync(ct); // ← FALTABA: sin esto los datos no se persisten
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(ct); // ← FALTABA
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Orders.AnyAsync(o => o.Id == id, ct);
    }
}
using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;

namespace PaymentService.Infrastructure.Persistence;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet;

    public Repository(PaymentDbContext context)
    {
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);
}

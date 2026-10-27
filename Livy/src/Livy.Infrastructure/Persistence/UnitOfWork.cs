using Livy.Domain.Interfaces;

namespace Livy.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly LivyDbContext _context;

    public UnitOfWork(LivyDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

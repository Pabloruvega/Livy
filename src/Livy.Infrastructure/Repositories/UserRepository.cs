// Infrastructure/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using Livy.Application.Interfaces;
using Livy.Domain.Entities;
using Livy.Infrastructure.Persistence;

namespace Livy.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly LivyDbContext _context;

    public UserRepository(LivyDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Users.FindAsync(new object[] { id }, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default)
    {
        // Devuelve true si NO existe ningún usuario con ese email
        return !await _context.Users.AnyAsync(u => u.Email == email, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct); // ← FALTABA
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct); // ← FALTABA
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Users.AsNoTracking().ToListAsync(ct);
    }
}
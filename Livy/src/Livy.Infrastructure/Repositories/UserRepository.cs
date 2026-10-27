// Infrastructure/Repositories/UserRepository.cs
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Entities;
using Livy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livy.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly LivyDbContext _ctx;

    public UserRepository(LivyDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _ctx.Users
               .AsNoTracking()
               .FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default)
        => !await _ctx.Users.AnyAsync(u => u.Email == email, ct);


}
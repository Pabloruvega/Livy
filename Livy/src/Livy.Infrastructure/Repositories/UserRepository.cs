// Infrastructure/Repositories/UserRepository.cs
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Entities;
using Livy.Domain.ValueObjects;
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

    // Se compara la propiedad completa contra el Value Object: EF Core aplica el
    // value converter al parámetro. u.Email.Value no se puede traducir a SQL.
    public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
        => await _ctx.Users
               .AsNoTracking()
               .FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken ct = default)
        => !await _ctx.Users.AnyAsync(u => u.Email == email, ct);


}
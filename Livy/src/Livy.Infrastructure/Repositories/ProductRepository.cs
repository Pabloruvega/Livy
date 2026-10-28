// Infrastructure/Repositories/ProductRepository.cs
using Livy.Application.Contracts.Persistence;
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

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken ct = default)
        => await _ctx.Products
               .AsNoTracking()
               .Where(p => p.IsActive)
               .ToListAsync(ct);

    // EF Core traduce Contains a instr(), que en SQLite distingue mayúsculas. LIKE no las distingue
    // (solo para letras ASCII: "ó" y "Ó" siguen siendo distintas). Los comodines del término se
    // escapan para que "%" o "_" se busquen como texto.
    public async Task<IEnumerable<Product>> SearchByNameAsync(
        string term, bool includeInactive, CancellationToken ct = default)
    {
        var pattern = $"%{EscapeLikePattern(term)}%";

        var query = _ctx.Products
            .AsNoTracking()
            .Where(p => EF.Functions.Like(p.Name, pattern, LikeEscapeCharacter));

        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        return await query.ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Products.AnyAsync(p => p.Id == id, ct);

    private const string LikeEscapeCharacter = "\\";

    private static string EscapeLikePattern(string value)
        => value.Replace(LikeEscapeCharacter, LikeEscapeCharacter + LikeEscapeCharacter)
                .Replace("%", LikeEscapeCharacter + "%")
                .Replace("_", LikeEscapeCharacter + "_");
}
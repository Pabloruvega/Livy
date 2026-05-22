// Infrastructure/Persistence/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Livy.Domain.Entities;

namespace Livy.Infrastructure.Persistence;

public class LivyDbContext : DbContext
{
    public LivyDbContext(DbContextOptions<LivyDbContext> options)
        : base(options) { }

    // Un DbSet por tabla — reflejan el modelo relacional
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas las clases IEntityTypeConfiguration del assembly
        // automáticamente — no hace falta llamarlas una por una
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LivyDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

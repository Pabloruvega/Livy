// Infrastructure/Persistence/configurations/CategoryConfiguration.cs
using Livy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livy.Infrastructure.Persistence.Configurations; 

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    // GUIDs FIJOS — nunca usar Guid.NewGuid() en HasData()
    private static readonly Guid ElectronicaId =
        new Guid("a1b2c3d4-0000-0000-0000-000000000001");
    private static readonly Guid RopaId =
        new Guid("a1b2c3d4-0000-0000-0000-000000000002");
    private static readonly Guid HogarId =
        new Guid("a1b2c3d4-0000-0000-0000-000000000003");

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        // Relación inversa con Products
        builder.HasMany(c => c.Products)
               .WithOne()
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        // Seed de datos iniciales — GUIDs hardcodeados obligatoriamente
        builder.HasData(
            new { Id = ElectronicaId, Name = "Electrónica" },
            new { Id = RopaId,        Name = "Ropa" },
            new { Id = HogarId,       Name = "Hogar" }
        );
    }
}
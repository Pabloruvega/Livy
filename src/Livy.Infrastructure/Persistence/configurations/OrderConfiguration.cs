using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Livy.Domain.Entities;

namespace Livy.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.UserId).IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.Total)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        // Guardar el enum como string legible, no como número
        builder.Property(o => o.Status)
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.HasMany(o => o.Items)
               .WithOne()  
               .HasForeignKey(i => i.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}




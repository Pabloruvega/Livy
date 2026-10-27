using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Livy.Domain.Entities;
using Livy.Domain.ValueObjects;

namespace Livy.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        // Value Object Email guardado como string en la misma columna "Email"
        builder.Property(u => u.Email)
               .HasConversion(e => e.Value, v => Email.Create(v))
               .HasColumnName("Email")
               .IsRequired()
               .HasMaxLength(250);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(200);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).IsRequired().HasMaxLength(20);
        builder.Property(u => u.CreatedAt).IsRequired();

        // Email debe ser único en la tabla
        builder.HasIndex(u => u.Email).IsUnique();
    }
}


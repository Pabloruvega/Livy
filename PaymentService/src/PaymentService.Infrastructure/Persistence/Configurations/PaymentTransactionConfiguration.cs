using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Infrastructure.Persistence.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.OrderId).IsRequired();

        // Money es un Value Object inmutable — se guarda como decimal simple.
        builder.Property(t => t.Amount)
               .HasConversion(m => m.Value, v => new Money(v))
               .HasColumnName("Amount")
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(t => t.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(t => t.TransactionCode)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(t => t.ProcessedAt).IsRequired();

        // Una transacción por orden (idempotencia a nivel de datos) y código único.
        builder.HasIndex(t => t.OrderId).IsUnique();
        builder.HasIndex(t => t.TransactionCode).IsUnique();
    }
}

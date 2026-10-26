using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Contracts.Persistence;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure.Repositories;

public class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransactionRepository
{
    private readonly PaymentDbContext _ctx;

    public PaymentTransactionRepository(PaymentDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }

    public async Task<PaymentTransaction?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        => await _ctx.Transactions.FirstOrDefaultAsync(t => t.OrderId == orderId, ct);
}

using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;

namespace PaymentService.Application.Contracts.Persistence;

public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
{
    Task<PaymentTransaction?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}

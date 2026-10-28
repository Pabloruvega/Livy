// Application/Contracts/Persistence/IOrderRepository.cs
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;

namespace Livy.Application.Contracts.Persistence;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> HasOrdersForProductAsync(Guid productId, CancellationToken ct = default);
}
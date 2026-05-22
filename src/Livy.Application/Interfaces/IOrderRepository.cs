// Application/Interfaces/IOrderRepository.cs
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;

namespace Livy.Application.Interfaces;


public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
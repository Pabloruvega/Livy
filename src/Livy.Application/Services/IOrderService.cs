// Application/Services/IOrderService.cs
using Livy.Application.DTOs.Order;

namespace Livy.Application.Services;

public interface IOrderService
{
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<OrderDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);
}
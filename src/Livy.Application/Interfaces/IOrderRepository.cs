using Livy.Domain.Entities;

namespace Livy.Application.Interfaces;

public interface IOrderRepository
{
    // Obtener una orden por ID, usualmente incluyendo sus OrderItems
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Obtener todas las órdenes de un usuario específico
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    // Guardar una nueva orden
    Task AddAsync(Order order, CancellationToken ct = default);

    // Actualizar el estado de una orden (ej. de Pending a Shipped)
    Task UpdateAsync(Order order, CancellationToken ct = default);
    
    // Verificar si existe una orden
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
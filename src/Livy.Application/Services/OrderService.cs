// Application/Services/OrderService.cs
using Livy.Application.DTOs.Order;
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository   _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        ILogger<OrderService> logger)
    {
        _orderRepo   = orderRepo;
        _productRepo = productRepo;
        _logger      = logger;
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _orderRepo.GetByIdAsync(id, ct);
        return order is null ? null : MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var orders = await _orderRepo.GetByUserIdAsync(userId, ct);
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creando orden para usuario {UserId}.", dto.UserId);

        var order = new Order(dto.UserId);

        foreach (var item in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId, ct)
                ?? throw new ProductNotFoundException(item.ProductId);

            order.AddItem(product, item.Quantity); // valida stock en el dominio
            await _productRepo.UpdateAsync(product, ct); // persiste reducción de stock
        }

        await _orderRepo.AddAsync(order, ct);
        _logger.LogInformation("Orden {Id} creada. Total: {Total}.", order.Id, order.Total);
        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order o) => new()
    {
        Id        = o.Id,
        UserId    = o.UserId,
        Status    = o.Status.ToString(),
        Total     = o.Total,
        CreatedAt = o.CreatedAt,
        Items     = o.Items.Select(i => new OrderItemDto
        {
            Id        = i.Id,
            ProductId = i.ProductId,
            UnitPrice = i.UnitPrice,
            Quantity  = i.Quantity,
            Subtotal  = i.Subtotal
        }).ToList()
    };
}
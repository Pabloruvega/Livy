// Application/UseCases/Orders/Queries/GetOrderById/GetOrderByIdHandler.cs
using Livy.Application.Common;
using Livy.Application.DTOs.Order;
using Livy.Application.Interfaces;
using Livy.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Livy.Application.UseCases.Orders.Queries.GetOrderById;

public class GetOrderByIdHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _repo;
    private readonly ILogger<GetOrderByIdHandler> _logger;

    public GetOrderByIdHandler(IOrderRepository repo, ILogger<GetOrderByIdHandler> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<OrderDto> HandleAsync(
        GetOrderByIdQuery query, CancellationToken ct = default)
    {
        _logger.LogInformation("Ejecutando GetOrderByIdQuery para Id {Id}.", query.OrderId);

        var order = await _repo.GetByIdAsync(query.OrderId, ct)
            ?? throw new DomainException($"Orden con Id '{query.OrderId}' no encontrada.");

        return new OrderDto
        {
            Id        = order.Id,
            UserId    = order.UserId,
            Status    = order.Status.ToString(),
            Total     = order.Total,
            CreatedAt = order.CreatedAt,
            Items     = order.Items.Select(i => new OrderItemDto
            {
                Id        = i.Id,
                ProductId = i.ProductId,
                UnitPrice = i.UnitPrice,
                Quantity  = i.Quantity,
                Subtotal  = i.Subtotal
            }).ToList()
        };
    }
}
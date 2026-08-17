// Application/Features/Orders/Queries/GetOrderById/GetOrderByIdHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Order;
using Livy.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _repo;
    private readonly ILogger<GetOrderByIdHandler> _logger;

    public GetOrderByIdHandler(IOrderRepository repo, ILogger<GetOrderByIdHandler> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando GetOrderByIdQuery para Id {Id}.", request.OrderId);

        var order = await _repo.GetByIdWithItemsAsync(request.OrderId, ct)
            ?? throw new OrderNotFoundException(request.OrderId);

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
// Application/Features/Orders/Commands/CreateOrder/CreateOrderCommand.cs
using Livy.Application.DTOs.Order;
using MediatR;

namespace Livy.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid UserId,
    List<CreateOrderItemCommand> Items
) : IRequest<OrderDto>;

public record CreateOrderItemCommand(Guid ProductId, int Quantity);
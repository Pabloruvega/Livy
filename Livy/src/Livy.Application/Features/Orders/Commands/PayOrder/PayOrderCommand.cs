// Application/Features/Orders/Commands/PayOrder/PayOrderCommand.cs
using Livy.Application.DTOs.Order;
using MediatR;

namespace Livy.Application.Features.Orders.Commands.PayOrder;

// Reintenta el pago de una orden que quedó Pending (por ejemplo, PaymentService caído).
// RequestingUserId e IsAdmin salen del token.
public record PayOrderCommand(Guid OrderId, Guid RequestingUserId, bool IsAdmin) : IRequest<OrderDto>;

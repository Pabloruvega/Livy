// Application/Features/Orders/Queries/GetOrderById/GetOrderByIdQuery.cs
using Livy.Application.DTOs.Order;
using MediatR;

namespace Livy.Application.Features.Orders.Queries.GetOrderById;

// RequestingUserId e IsAdmin salen del token: el dueño ve su orden, el Admin ve cualquiera.
public record GetOrderByIdQuery(Guid OrderId, Guid RequestingUserId, bool IsAdmin) : IRequest<OrderDto>;

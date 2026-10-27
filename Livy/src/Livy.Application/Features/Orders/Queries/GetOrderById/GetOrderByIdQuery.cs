// Application/Features/Orders/Queries/GetOrderById/GetOrderByIdQuery.cs
using Livy.Application.DTOs.Order;
using MediatR;

namespace Livy.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;
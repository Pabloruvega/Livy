// Application/Features/Orders/OrderMappings.cs
using Livy.Application.DTOs.Order;
using Livy.Domain.Entities;

namespace Livy.Application.Features.Orders;

// Único mapeo Order → OrderDto, compartido por los handlers de Orders.
// paymentMessage solo se informa en los casos de uso que acaban de procesar un pago.
public static class OrderMappings
{
    public static OrderDto ToDto(this Order order, string? paymentMessage = null) => new()
    {
        Id                     = order.Id,
        UserId                 = order.UserId,
        Status                 = order.Status.ToString(),
        Total                  = order.Total,
        CreatedAt              = order.CreatedAt,
        PaymentTransactionCode = order.PaymentTransactionCode,
        PaymentMessage         = paymentMessage,
        Items                  = order.Items.Select(i => new OrderItemDto
        {
            Id        = i.Id,
            ProductId = i.ProductId,
            UnitPrice = i.UnitPrice,
            Quantity  = i.Quantity,
            Subtotal  = i.Subtotal
        }).ToList()
    };
}

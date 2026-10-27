// Application/Features/Orders/Commands/CreateOrder/CreateOrderHandler.cs
using Livy.Application.Contracts.External;
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Order;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository   _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork        _unitOfWork;
    private readonly IPaymentClient     _paymentClient;
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork,
        IPaymentClient paymentClient,
        ILogger<CreateOrderHandler> logger)
    {
        _orderRepo     = orderRepo;
        _productRepo   = productRepo;
        _unitOfWork    = unitOfWork;
        _paymentClient = paymentClient;
        _logger        = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando CreateOrderCommand para usuario {UserId}.", request.UserId);

        var order = new Order(request.UserId);
        var reserved = new List<(Product Product, int Quantity)>();

        foreach (var item in request.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId, ct)
                ?? throw new ProductNotFoundException(item.ProductId);

            order.AddItem(product, item.Quantity); // valida stock en el dominio
            reserved.Add((product, item.Quantity));
        }

        var paymentMessage = await ProcessPaymentAsync(order, reserved, ct);

        foreach (var (product, _) in reserved)
            await _productRepo.UpdateAsync(product, ct);

        await _orderRepo.AddAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct); // persiste orden + stock (+ resultado del pago) en una sola transacción

        _logger.LogInformation("Orden {Id} creada con estado {Status}. Total: {Total}.", order.Id, order.Status, order.Total);

        return new OrderDto
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

    // Llama al PaymentService y hace que la orden actúe según la respuesta:
    // - Approved  -> la orden queda Paid.
    // - Rejected  -> la orden queda PaymentRejected y se devuelve el stock reservado.
    // - sin respuesta (caído/timeout, ya reintentado por la política de Polly) -> la
    //   orden queda Pending (el stock se mantiene reservado) y se avisa al cliente
    //   que puede reintentar la confirmación más tarde.
    private async Task<string> ProcessPaymentAsync(
        Order order, List<(Product Product, int Quantity)> reserved, CancellationToken ct)
    {
        var result = await _paymentClient.ProcessPaymentAsync(order.Id, order.Total, ct);

        if (result is null)
        {
            _logger.LogWarning("PaymentService no respondió para la orden {OrderId}; queda Pending.", order.Id);
            return "No se pudo confirmar el pago en este momento. La orden quedó pendiente, podés reintentarlo más tarde.";
        }

        if (result.Status == "Approved")
        {
            order.MarkAsPaid(result.TransactionCode);
            return "Pago aprobado.";
        }

        order.RejectPayment(result.TransactionCode);
        foreach (var (product, quantity) in reserved)
            product.RestoreStock(quantity);

        return "Pago rechazado: el monto supera el límite permitido.";
    }
}
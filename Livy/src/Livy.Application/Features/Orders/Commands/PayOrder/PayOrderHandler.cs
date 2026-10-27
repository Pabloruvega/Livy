// Application/Features/Orders/Commands/PayOrder/PayOrderHandler.cs
using Livy.Application.Common.Exceptions;
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Order;
using Livy.Application.Features.Orders.Common;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Orders.Commands.PayOrder;

public class PayOrderHandler : IRequestHandler<PayOrderCommand, OrderDto>
{
    private readonly IOrderRepository      _orderRepo;
    private readonly IProductRepository    _productRepo;
    private readonly IUnitOfWork           _unitOfWork;
    private readonly OrderPaymentProcessor _paymentProcessor;
    private readonly ILogger<PayOrderHandler> _logger;

    public PayOrderHandler(
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork,
        OrderPaymentProcessor paymentProcessor,
        ILogger<PayOrderHandler> logger)
    {
        _orderRepo        = orderRepo;
        _productRepo      = productRepo;
        _unitOfWork       = unitOfWork;
        _paymentProcessor = paymentProcessor;
        _logger           = logger;
    }

    public async Task<OrderDto> Handle(PayOrderCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando PayOrderCommand para la orden {OrderId}.", request.OrderId);

        var order = await _orderRepo.GetByIdWithItemsAsync(request.OrderId, ct)
            ?? throw new OrderNotFoundException(request.OrderId);

        if (!request.IsAdmin && !order.IsOwnedBy(request.RequestingUserId))
            throw new ForbiddenAccessException("No tenés permiso para pagar esta orden.");

        // Productos de los ítems: si el pago es rechazado hay que devolverles el stock.
        var products = new Dictionary<Guid, Product>();
        foreach (var item in order.Items)
        {
            if (products.ContainsKey(item.ProductId))
                continue;

            products[item.ProductId] = await _productRepo.GetByIdAsync(item.ProductId, ct)
                ?? throw new ProductNotFoundException(item.ProductId);
        }

        // Si la orden no está Pending, la guarda de la entidad lanza DomainException.
        var paymentMessage = await _paymentProcessor.ProcessAsync(order, products, ct);

        await _unitOfWork.SaveChangesAsync(ct); // orden y productos están trackeados por EF

        _logger.LogInformation("Orden {Id} procesada con estado {Status}.", order.Id, order.Status);

        return order.ToDto(paymentMessage);
    }
}

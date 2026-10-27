// Application/Features/Orders/Commands/CreateOrder/CreateOrderHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Order;
using Livy.Application.Features.Orders.Common;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository      _orderRepo;
    private readonly IProductRepository    _productRepo;
    private readonly IUnitOfWork           _unitOfWork;
    private readonly OrderPaymentProcessor _paymentProcessor;
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork,
        OrderPaymentProcessor paymentProcessor,
        ILogger<CreateOrderHandler> logger)
    {
        _orderRepo        = orderRepo;
        _productRepo      = productRepo;
        _unitOfWork       = unitOfWork;
        _paymentProcessor = paymentProcessor;
        _logger           = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando CreateOrderCommand para usuario {UserId}.", request.UserId);

        var order = new Order(request.UserId);
        var products = new Dictionary<Guid, Product>();

        foreach (var item in request.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId, ct)
                ?? throw new ProductNotFoundException(item.ProductId);

            order.AddItem(product, item.Quantity); // valida stock en el dominio
            products[product.Id] = product;
        }

        var paymentMessage = await _paymentProcessor.ProcessAsync(order, products, ct);

        foreach (var product in products.Values)
            await _productRepo.UpdateAsync(product, ct);

        await _orderRepo.AddAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct); // persiste orden + stock (+ resultado del pago) en una sola transacción

        _logger.LogInformation("Orden {Id} creada con estado {Status}. Total: {Total}.", order.Id, order.Status, order.Total);

        return order.ToDto(paymentMessage);
    }
}

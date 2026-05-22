// Application/Features/Orders/Commands/CreateOrder/CreateOrderHandler.cs
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
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrderHandler> logger)
    {
        _orderRepo   = orderRepo;
        _productRepo = productRepo;
        _unitOfWork  = unitOfWork;
        _logger      = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Ejecutando CreateOrderCommand para usuario {UserId}.", request.UserId);

        var order = new Order(request.UserId);

        foreach (var item in request.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId, ct)
                ?? throw new ProductNotFoundException(item.ProductId);

            order.AddItem(product, item.Quantity); // valida stock en el dominio
            await _productRepo.UpdateAsync(product, ct);
        }

        await _orderRepo.AddAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct); // persiste orden + stock en una sola transacción

        _logger.LogInformation("Orden {Id} creada. Total: {Total}.", order.Id, order.Total);

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
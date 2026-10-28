// WebApi/Controllers/OrdersController.cs
using Livy.Application.DTOs.Order;
using Livy.Application.Features.Orders.Commands.CreateOrder;
using Livy.Application.Features.Orders.Commands.PayOrder;
using Livy.Application.Features.Orders.Queries.GetOrderById;
using Livy.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livy.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene una orden por Id con sus ítems.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(
            new GetOrderByIdQuery(id, User.GetUserId(), User.IsInRole("Admin")), ct));

    /// <summary>Crea una nueva orden y descuenta el stock automáticamente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var command = new CreateOrderCommand(
            User.GetUserId(),
            dto.Items.Select(i => new CreateOrderItemCommand(i.ProductId, i.Quantity)).ToList());
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Reintenta el pago de una orden Pending (por ejemplo, si PaymentService estaba caído).</summary>
    [HttpPost("{id:guid}/pay")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pay(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(
            new PayOrderCommand(id, User.GetUserId(), User.IsInRole("Admin")), ct));
}
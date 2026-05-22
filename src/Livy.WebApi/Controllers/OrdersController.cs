// WebApi/Controllers/OrdersController.cs
using Livy.Application.Common;
using Livy.Application.DTOs.Order;
using Livy.Application.UseCases.Orders.Commands.CreateOrder;
using Livy.Application.UseCases.Orders.Queries.GetOrderById;
using Microsoft.AspNetCore.Mvc;

namespace Livy.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly ICommandHandler<CreateOrderCommand, OrderDto>  _createHandler;
    private readonly IQueryHandler<GetOrderByIdQuery, OrderDto>     _getByIdHandler;

    public OrdersController(
        ICommandHandler<CreateOrderCommand, OrderDto>  createHandler,
        IQueryHandler<GetOrderByIdQuery, OrderDto>     getByIdHandler)
    {
        _createHandler  = createHandler;
        _getByIdHandler = getByIdHandler;
    }

  
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _getByIdHandler.HandleAsync(new GetOrderByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var command = new CreateOrderCommand(
            dto.UserId,
            dto.Items.Select(i => new CreateOrderItemCommand(i.ProductId, i.Quantity)).ToList()
        );

        var created = await _createHandler.HandleAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
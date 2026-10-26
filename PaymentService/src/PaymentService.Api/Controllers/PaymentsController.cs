using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTOs;
using PaymentService.Application.Features.Payments.Commands.ProcessPayment;
using PaymentService.Application.Features.Payments.Queries.GetPaymentByOrderId;

namespace PaymentService.Api.Controllers;

[ApiController]
[Route("api/payments")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("process")]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Process([FromBody] ProcessPaymentCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpGet("order/{orderId:guid}")]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetPaymentByOrderIdQuery(orderId), ct));
}

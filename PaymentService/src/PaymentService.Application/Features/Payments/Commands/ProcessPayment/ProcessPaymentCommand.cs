using PaymentService.Application.DTOs;
using MediatR;

namespace PaymentService.Application.Features.Payments.Commands.ProcessPayment;

public record ProcessPaymentCommand(Guid OrderId, decimal Amount) : IRequest<PaymentResultDto>;

using PaymentService.Application.DTOs;
using MediatR;

namespace PaymentService.Application.Features.Payments.Queries.GetPaymentByOrderId;

public record GetPaymentByOrderIdQuery(Guid OrderId) : IRequest<PaymentResultDto>;

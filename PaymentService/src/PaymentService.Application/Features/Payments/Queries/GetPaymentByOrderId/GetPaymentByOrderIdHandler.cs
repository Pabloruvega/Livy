using MediatR;
using PaymentService.Application.Contracts.Persistence;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Application.Features.Payments.Queries.GetPaymentByOrderId;

public class GetPaymentByOrderIdHandler : IRequestHandler<GetPaymentByOrderIdQuery, PaymentResultDto>
{
    private readonly IPaymentTransactionRepository _repo;

    public GetPaymentByOrderIdHandler(IPaymentTransactionRepository repo) => _repo = repo;

    public async Task<PaymentResultDto> Handle(GetPaymentByOrderIdQuery request, CancellationToken ct)
    {
        var transaction = await _repo.GetByOrderIdAsync(request.OrderId, ct)
            ?? throw new PaymentTransactionNotFoundException(request.OrderId);

        return PaymentResultDto.FromEntity(transaction);
    }
}

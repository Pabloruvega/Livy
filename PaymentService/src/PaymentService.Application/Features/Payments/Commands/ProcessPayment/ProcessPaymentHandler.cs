using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Contracts.Persistence;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;

namespace PaymentService.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, PaymentResultDto>
{
    private readonly IPaymentTransactionRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly decimal _approvalLimit;
    private readonly ILogger<ProcessPaymentHandler> _logger;

    public ProcessPaymentHandler(
        IPaymentTransactionRepository repo,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<ProcessPaymentHandler> logger)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
        _logger = logger;

        // GetValue<decimal> parsea con cultura invariante, a diferencia de decimal.Parse
        // directo sobre config[...] (rompería si el hilo corre con una cultura donde
        // "." no es el separador decimal).
        _approvalLimit = configuration.GetValue<decimal?>("PaymentRules:ApprovalLimit")
            ?? throw new InvalidOperationException(
                "Falta configurar 'PaymentRules:ApprovalLimit' en appsettings.json.");
    }

    public async Task<PaymentResultDto> Handle(ProcessPaymentCommand request, CancellationToken ct)
    {
        // Idempotencia: si el e-commerce reintenta la llamada (timeout, retry de Polly, etc.)
        // para la misma orden, devolvemos la transacción ya existente en vez de duplicarla.
        var existing = await _repo.GetByOrderIdAsync(request.OrderId, ct);
        if (existing is not null)
        {
            _logger.LogInformation(
                "Orden {OrderId} ya tenía una transacción procesada ({Status}); no se duplica.",
                request.OrderId, existing.Status);
            return PaymentResultDto.FromEntity(existing);
        }

        var transaction = PaymentTransaction.Process(request.OrderId, request.Amount, _approvalLimit);

        await _repo.AddAsync(transaction, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Pago de la orden {OrderId} procesado como {Status} ({Code}).",
            transaction.OrderId, transaction.Status, transaction.TransactionCode);

        return PaymentResultDto.FromEntity(transaction);
    }
}

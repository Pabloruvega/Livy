// Application/Features/Orders/Common/OrderPaymentProcessor.cs
using Livy.Application.Contracts.External;
using Livy.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Orders.Common;

// Único lugar que llama al PaymentService y aplica el resultado a la orden.
// Lo usan CreateOrder (primer intento) y PayOrder (reintento). No persiste:
// cada handler guarda con IUnitOfWork en su propia transacción.
public class OrderPaymentProcessor
{
    private readonly IPaymentClient _paymentClient;
    private readonly ILogger<OrderPaymentProcessor> _logger;

    public OrderPaymentProcessor(IPaymentClient paymentClient, ILogger<OrderPaymentProcessor> logger)
    {
        _paymentClient = paymentClient;
        _logger        = logger;
    }

    // products: los productos de los ítems de la orden, por Id (se usan para
    // devolver el stock si el pago es rechazado).
    // - Approved -> la orden queda Paid.
    // - Rejected -> la orden queda PaymentRejected y se devuelve el stock de cada ítem.
    // - sin respuesta (caído/timeout, ya reintentado por la política de Polly) -> la
    //   orden queda Pending (el stock se mantiene reservado) y se avisa al cliente
    //   que puede reintentar el pago más tarde.
    public async Task<string> ProcessAsync(
        Order order, IReadOnlyDictionary<Guid, Product> products, CancellationToken ct)
    {
        order.EnsureCanProcessPayment(); // DomainException si la orden no está Pending

        var result = await _paymentClient.ProcessPaymentAsync(order.Id, order.Total, ct);

        if (result is null)
        {
            _logger.LogWarning("PaymentService no respondió para la orden {OrderId}; queda Pending.", order.Id);
            return "No se pudo confirmar el pago en este momento. La orden quedó pendiente, podés reintentarlo más tarde.";
        }

        if (result.Outcome == PaymentOutcome.Approved)
        {
            order.MarkAsPaid(result.TransactionCode);
            return "Pago aprobado.";
        }

        order.RejectPayment(result.TransactionCode);
        foreach (var item in order.Items)
            products[item.ProductId].RestoreStock(item.Quantity);

        return "El pago fue rechazado por el servicio de pagos.";
    }
}

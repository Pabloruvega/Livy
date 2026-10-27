namespace Livy.Application.Contracts.External;

// Resultado del PaymentService mapeado a lo que Application necesita saber —
// nunca se expone el DTO/response HTTP crudo de ese servicio.
public enum PaymentOutcome
{
    Approved,
    Rejected
}

public record PaymentResult(PaymentOutcome Outcome, string TransactionCode);

public interface IPaymentClient
{
    // Devuelve null si el PaymentService no pudo ser contactado (caído, timeout,
    // agotó los reintentos) o respondió algo que no se puede interpretar — no es una
    // excepción porque es un resultado esperado en un flujo de integración entre
    // servicios, no un error de programación.
    Task<PaymentResult?> ProcessPaymentAsync(Guid orderId, decimal amount, CancellationToken ct = default);
}

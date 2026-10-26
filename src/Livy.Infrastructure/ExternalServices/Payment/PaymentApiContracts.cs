namespace Livy.Infrastructure.ExternalServices.Payment;

// DTOs propios del e-commerce para hablar con el PaymentService por HTTP.
// No son las mismas clases que usa PaymentService internamente — cada solución
// tiene los suyos, solo comparten forma (mismos nombres de campo en JSON).
internal record ProcessPaymentApiRequest(Guid OrderId, decimal Amount);

internal record ProcessPaymentApiResponse(
    Guid TransactionId,
    Guid OrderId,
    string Status,
    string TransactionCode,
    decimal Amount,
    DateTime ProcessedAt);

using Livy.Application.Contracts.External;
using Microsoft.Extensions.Logging;
using Polly.Timeout;
using System.Net.Http.Json;

namespace Livy.Infrastructure.ExternalServices.Payment;

public class PaymentClient : IPaymentClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentClient> _logger;

    public PaymentClient(HttpClient httpClient, ILogger<PaymentClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PaymentResult?> ProcessPaymentAsync(Guid orderId, decimal amount, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/payments/process", new ProcessPaymentApiRequest(orderId, amount), ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "PaymentService respondió {StatusCode} para la orden {OrderId}.",
                    response.StatusCode, orderId);
                return null;
            }

            var body = await response.Content.ReadFromJsonAsync<ProcessPaymentApiResponse>(ct);
            if (body is null)
                return null;

            // Traducción del string del JSON al resultado tipado de Application.
            PaymentOutcome? outcome = body.Status switch
            {
                "Approved" => PaymentOutcome.Approved,
                "Rejected" => PaymentOutcome.Rejected,
                _          => null
            };

            if (outcome is null)
            {
                _logger.LogWarning(
                    "PaymentService devolvió un estado desconocido '{Status}' para la orden {OrderId}; se trata como no disponible.",
                    body.Status, orderId);
                return null;
            }

            return new PaymentResult(outcome.Value, body.TransactionCode);
        }
        catch (Exception ex) when (
            ex is HttpRequestException            // conexión rechazada / error de red en el último intento
               or TimeoutRejectedException        // el último intento superó el timeout por intento de Polly
            || (ex is OperationCanceledException  // timeout total de HttpClient (TaskCanceledException)...
                && !ct.IsCancellationRequested))  // ...pero NO si el que canceló fue el cliente: esa se propaga
        {
            // PaymentService caído, o se agotaron los reintentos de la política de Polly.
            _logger.LogError(ex, "No se pudo contactar a PaymentService para la orden {OrderId}.", orderId);
            return null;
        }
    }
}

using Livy.Application.Contracts.External;
using Microsoft.Extensions.Logging;
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
            return body is null ? null : new PaymentResult(body.Status, body.TransactionCode);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // PaymentService caído, o se agotaron los reintentos de la política de Polly.
            _logger.LogError(ex, "No se pudo contactar a PaymentService para la orden {OrderId}.", orderId);
            return null;
        }
    }
}

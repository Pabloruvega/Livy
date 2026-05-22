// WebApi/Middleware/ErrorHandlingMiddleware.cs
using Livy.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Livy.WebApi.Middleware;

/// <summary>
/// Middleware global que captura todas las excepciones no manejadas,
/// las loguea y devuelve una respuesta JSON consistente al cliente.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            // Excepciones del dominio → 400 Bad Request
            DomainException      => (HttpStatusCode.BadRequest, ex.Message),
            ArgumentException    => (HttpStatusCode.BadRequest, ex.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
            // Cualquier otra excepción → 500 Internal Server Error
            _ => (HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor.")
        };

        // Loguear con nivel apropiado según el tipo
        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(ex, "Error no controlado en {Path}", context.Request.Path);
        else
            _logger.LogWarning("Error de dominio en {Path}: {Message}", context.Request.Path, ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var response = new
        {
            status  = (int)statusCode,
            error   = message,
            path    = context.Request.Path.ToString(),
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
// WebApi/Middleware/ErrorHandlingMiddleware.cs
using Livy.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Livy.WebApi.Middleware;


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
        // Errores de FluentValidation (Commands/Queries de MediatR) → 400 con errores por campo
        if (ex is FluentValidation.ValidationException validationEx)
        {
            await WriteValidationErrorAsync(context, validationEx);
            return;
        }

        var (statusCode, message) = ex switch
        {
            // Recurso no encontrado → 404 Not Found (chequear ANTES que DomainException,
            // porque NotFoundException hereda de DomainException)
            NotFoundException    => (HttpStatusCode.NotFound, ex.Message),
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

    private async Task WriteValidationErrorAsync(HttpContext context, FluentValidation.ValidationException ex)
    {
        _logger.LogWarning("Error de validación en {Path}: {Errors}",
            context.Request.Path, string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));

        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)HttpStatusCode.BadRequest;

        var response = new
        {
            status  = (int)HttpStatusCode.BadRequest,
            errors,
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
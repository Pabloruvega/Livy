// WebApi/ExceptionHandling/GlobalExceptionHandler.cs
using Livy.Application.Common.Exceptions;
using Livy.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Livy.WebApi.ExceptionHandling;

// Los apuntes lo ubican en Infrastructure/Middleware, pero queda en WebApi (Presentation):
// depende de HttpContext y ProblemDetails (ASP.NET Core), y la Unidad 1 ubica los
// "filtros de excepción" en Presentation. Infrastructure no tiene por qué conocer HTTP.
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger                = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = CreateProblemDetails(exception);
        problem.Instance              = httpContext.Request.Path;
        // Mismo criterio que el framework (401/403, 400 automáticas) → un solo formato de traceId
        problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        var method = httpContext.Request.Method;
        var path   = httpContext.Request.Path;
        var status = problem.Status!.Value;

        // 500 → excepción completa (stacktrace) solo en el log, nunca al cliente
        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "{Method} {Path} → {StatusCode}: error no controlado", method, path, status);
        else
            _logger.LogWarning("{Method} {Path} → {StatusCode}: {Detail}", method, path, status, GetLogDetail(problem));

        httpContext.Response.StatusCode = status;

        var written = await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext    = httpContext,
            ProblemDetails = problem,
            Exception      = exception
        });

        // El writer por defecto se niega si el header Accept no admite JSON; igual respondemos en RFC 7807
        if (!written)
            await httpContext.Response.WriteAsJsonAsync(
                problem, problem.GetType(), options: null, contentType: "application/problem+json", cancellationToken);

        return true;
    }

    // De lo más específico a lo más general: NotFoundException hereda de DomainException.
    private static ProblemDetails CreateProblemDetails(Exception exception) => exception switch
    {
        FluentValidation.ValidationException ex => new ValidationProblemDetails(
            ex.Errors
              .GroupBy(e => e.PropertyName)
              .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
        {
            // Sin detail: los mensajes ya van en "errors"
            Status = StatusCodes.Status400BadRequest,
            Title  = "Validation failed"
        },
        NotFoundException => new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title  = "Resource not found",
            Detail = exception.Message
        },
        ForbiddenAccessException => new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title  = "Forbidden",
            Detail = exception.Message
        },
        DomainException => new ProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title  = "Business rule violated",
            Detail = exception.Message
        },
        _ => new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title  = "An unexpected error occurred"
        }
    };

    // La 400 de validación no tiene detail: para el log se arma con los errores por propiedad.
    private static string? GetLogDetail(ProblemDetails problem) => problem is ValidationProblemDetails validation
        ? string.Join("; ", validation.Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"))
        : problem.Detail;
}

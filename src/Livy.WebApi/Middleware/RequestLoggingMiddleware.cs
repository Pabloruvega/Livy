// WebApi/Middleware/RequestLoggingMiddleware.cs
using System.Diagnostics;

namespace Livy.WebApi.Middleware;

/// <summary>
/// Loguea cada request HTTP: método, path, status code y tiempo de ejecución.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(
            "➡ {Method} {Path} iniciado",
            context.Request.Method,
            context.Request.Path);

        await _next(context);

        sw.Stop();

        _logger.LogInformation(
            "✅ {Method} {Path} → {StatusCode} en {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds);
    }
}
namespace PaymentService.Api.Middleware;

// Este servicio no tiene usuarios/roles propios — lo único que lo llama es el
// e-commerce (server-to-server). En vez de sumar JWT bearer completo para eso,
// se protege con una clave compartida por header, seteada en appsettings de
// ambos proyectos.
public class ApiKeyMiddleware
{
    private const string HeaderName = "X-Api-Key";

    private readonly RequestDelegate _next;
    private readonly string _apiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration["Security:ApiKey"]
            ?? throw new InvalidOperationException("Falta configurar 'Security:ApiKey' en appsettings.json.");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Swagger queda accesible sin key para poder probar el servicio a mano.
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey) ||
            providedKey != _apiKey)
        {
            // Results.Problem escribe vía IProblemDetailsService → application/problem+json con traceId
            await Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: $"Header '{HeaderName}' ausente o inválido.",
                instance: context.Request.Path).ExecuteAsync(context);
            return;
        }

        await _next(context);
    }
}

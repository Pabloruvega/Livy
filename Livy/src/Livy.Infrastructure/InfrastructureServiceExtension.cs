// Infrastructure/InfrastructureServiceExtension.cs
using Livy.Application.Contracts.External;
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Interfaces;
using Livy.Infrastructure.ExternalServices.Payment;
using Livy.Infrastructure.Persistence;
using Livy.Infrastructure.Repositories;
using Livy.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Livy.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext — Scoped
        services.AddDbContext<LivyDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // IUnitOfWork — Scoped
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositorios — Scoped
        // Las interfaces ahora están en Application/Contracts/Persistence/
        services.AddScoped<IProductRepository,  ProductRepository>();
        services.AddScoped<IOrderRepository,    OrderRepository>();
        services.AddScoped<IUserRepository,     UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // JWT Token Service
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Resiliencia hacia PaymentService (valores en Services:Payment, con defaults si faltan).
        var timeoutPerAttempt = TimeSpan.FromSeconds(
            GetPositiveInt(configuration, "Services:Payment:TimeoutPerAttemptSeconds", 3));
        var retryCount = GetPositiveInt(configuration, "Services:Payment:RetryCount", 3);

        // Peor caso: (1 intento + N reintentos) × timeout por intento + suma de las esperas
        // 1, 2, 4... s. Con 3 s y 3 reintentos: 4 × 3 + (1 + 2 + 4) = 19 s.
        var worstCaseSeconds = (retryCount + 1) * timeoutPerAttempt.TotalSeconds
                             + Enumerable.Range(1, retryCount).Sum(attempt => GetRetryDelay(attempt).TotalSeconds);
        var totalTimeout = TimeSpan.FromSeconds(
            GetPositiveInt(configuration, "Services:Payment:TotalTimeoutSeconds", (int)Math.Ceiling(worstCaseSeconds) + 5));

        // Cliente HTTP tipado hacia PaymentService — vía IHttpClientFactory, nunca "new HttpClient()".
        // BaseAddress y ApiKey salen de appsettings (Services:Payment), no hardcodeados.
        services.AddHttpClient<IPaymentClient, PaymentClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["Services:Payment:BaseUrl"]!);
            // HttpClient.Timeout cubre la llamada COMPLETA (todos los intentos y esperas de
            // los handlers de Polly), por eso tiene que ser mayor que el peor caso.
            client.Timeout = totalTimeout;
            client.DefaultRequestHeaders.Add("X-Api-Key", configuration["Services:Payment:ApiKey"]);
        })
        // Orden de los handlers: el primero que se agrega es el de AFUERA.
        // Retry por fuera y timeout por dentro: cada intento corre dentro de su propio
        // TimeoutPolicy, así que el límite de 3 s se reinicia en cada intento y el retry ve
        // el TimeoutRejectedException de un intento lento como un fallo más para reintentar.
        // Al revés (timeout por fuera), el timeout sería uno solo para toda la operación
        // y cortaría en medio de los reintentos.
        .AddPolicyHandler((sp, _) => GetPaymentRetryPolicy(
            retryCount, sp.GetRequiredService<ILogger<PaymentClient>>()))
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(
            timeoutPerAttempt, TimeoutStrategy.Optimistic));

        return services;
    }

    // Reintenta ante errores transitorios (HttpRequestException, 5xx, 408) y ante el
    // TimeoutRejectedException que lanza el timeout por intento, con esperas 1, 2, 4... s.
    // No reintenta OperationCanceledException: esa viene del timeout total o de que el
    // cliente canceló el request, y en los dos casos no tiene sentido seguir intentando.
    private static IAsyncPolicy<HttpResponseMessage> GetPaymentRetryPolicy(int retryCount, ILogger logger) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                retryCount,
                GetRetryDelay,
                (outcome, delay, attempt, _) => logger.LogWarning(
                    "PaymentService: reintento {Attempt}/{RetryCount} en {Delay}s. Motivo: {Reason}.",
                    attempt, retryCount, delay.TotalSeconds,
                    outcome.Exception?.GetType().Name ?? $"HTTP {(int)outcome.Result.StatusCode}"));

    private static TimeSpan GetRetryDelay(int attempt) => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));

    private static int GetPositiveInt(IConfiguration configuration, string key, int defaultValue)
        => int.TryParse(configuration[key], out var value) && value > 0 ? value : defaultValue;
}
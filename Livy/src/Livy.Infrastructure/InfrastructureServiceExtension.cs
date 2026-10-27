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
using Polly;
using Polly.Extensions.Http;

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

        // Cliente HTTP tipado hacia PaymentService — vía IHttpClientFactory, nunca "new HttpClient()".
        // BaseAddress y ApiKey salen de appsettings (Services:Payment), no hardcodeados.
        services.AddHttpClient<IPaymentClient, PaymentClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["Services:Payment:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("X-Api-Key", configuration["Services:Payment:ApiKey"]);
        })
        .AddPolicyHandler(GetPaymentRetryPolicy());

        return services;
    }

    // Reintenta 3 veces con backoff exponencial ante errores transitorios (5xx, fallas de
    // red) o timeout — PaymentClient igual atrapa la excepción final si se agotan.
    private static IAsyncPolicy<HttpResponseMessage> GetPaymentRetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
}
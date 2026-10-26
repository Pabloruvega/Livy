using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Contracts.Persistence;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.Repositories;

namespace PaymentService.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("PaymentDb")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();

        return services;
    }
}

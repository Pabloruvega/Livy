// Infrastructure/InfrastructureServiceExtension.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Livy.Application.Interfaces;
using Livy.Infrastructure.Persistence;
using Livy.Infrastructure.Repositories;

namespace Livy.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext — Scoped: una instancia por request HTTP
        // NUNCA Singleton: EF Core no es thread-safe
        services.AddDbContext<LivyDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")));

        // Repositorios — Scoped: misma lifetime que el DbContext
        services.AddScoped<IProductRepository,  ProductRepository>();
        services.AddScoped<IOrderRepository,    OrderRepository>();
        services.AddScoped<IUserRepository,     UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>(); // ← FALTABA

        return services;
    }
}
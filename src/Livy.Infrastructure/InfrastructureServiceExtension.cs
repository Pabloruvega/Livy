// Infrastructure/InfrastructureServiceExtension.cs
using Livy.Application.Interfaces;
using Livy.Domain.Interfaces;
using Livy.Infrastructure.Persistence;
using Livy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Livy.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext — Scoped: una instancia por request HTTP
        services.AddDbContext<LivyDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")));

        // IUnitOfWork — Scoped: misma instancia que el DbContext
        // Permite que los Handlers persistan todos los cambios de una sola vez
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositorios — Scoped: misma lifetime que el DbContext
        services.AddScoped<IProductRepository,  ProductRepository>();
        services.AddScoped<IOrderRepository,    OrderRepository>();
        services.AddScoped<IUserRepository,     UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
// Infrastructure/InfrastructureServiceExtension.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.Services;
using Livy.Domain.Interfaces;
using Livy.Infrastructure.Persistence;
using Livy.Infrastructure.Repositories;
using Livy.Infrastructure.Services;
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

        return services;
    }
}
// Application/ApplicationServiceRegistration.cs
using Livy.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Livy.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Servicios de la capa Application — Scoped: uno por request HTTP
        services.AddScoped<IProductService,  ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService,     UserService>();
        services.AddScoped<IOrderService,    OrderService>();

        return services;
    }
}
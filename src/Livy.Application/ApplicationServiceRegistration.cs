// Application/ApplicationServiceRegistration.cs
using Livy.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Livy.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // MediatR escanea el assembly de Application y registra
        // automáticamente todos los IRequestHandler que encuentre.
        // No hay que registrar cada Handler manualmente.
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Services de User y Category (login/register no usan MediatR)
        services.AddScoped<IUserService,     UserService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
// Application/ApplicationServiceRegistration.cs
using FluentValidation;
using Livy.Application.Common.Behaviors;
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
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // ValidationBehavior corre ANTES que el Handler para cada Command/Query
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Registra todos los AbstractValidator<T> del assembly (uno por Command)
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Services de User y Category (login/register no usan MediatR)
        services.AddScoped<IUserService,     UserService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
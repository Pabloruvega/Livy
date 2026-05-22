using Microsoft.Extensions.DependencyInjection;

namespace Livy.Application.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Aquí irán los registros de Application
        // Por ejemplo: MediatR, Validators, AutoMapper, etc.

        return services;
    }
}

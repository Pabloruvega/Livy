using Livy.Application.Features.Users.Commands.PromoteToAdmin;
using Livy.Application.Features.Users.Commands.RegisterUser;
using Livy.Application.Features.Users.Queries.GetAllUsers;
using MediatR;

namespace Livy.WebApi;

// Sin esto, el registro público siempre deja Role="User" y los endpoints
// [Authorize(Roles = "Admin")] quedan inalcanzables. Corre una vez al arrancar
// la Api; si ya existe un Admin, no hace nada. Reutiliza RegisterUserCommand
// (valida email único, hashea password) en vez de duplicar esa lógica acá.
public static class DbSeeder
{
    public static async Task SeedAdminUserAsync(
        IMediator mediator, IConfiguration configuration, ILogger logger)
    {
        var users = await mediator.Send(new GetAllUsersQuery());
        if (users.Any(u => u.Role == "Admin"))
            return;

        // appsettings.json los deja vacíos: en desarrollo vienen de appsettings.Development.json.
        var email    = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException(
                "Falta configurar 'Admin:Email' / 'Admin:Password' (en desarrollo, appsettings.Development.json).");

        var admin = await mediator.Send(new RegisterUserCommand(email, "Admin", password));
        await mediator.Send(new PromoteToAdminCommand(admin.Id));

        logger.LogInformation("Usuario Admin sembrado con email {Email}.", email);
    }
}

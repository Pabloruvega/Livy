using Livy.Application.DTOs.User;
using Livy.Application.Services;

namespace Livy.WebApi;

// Sin esto, el registro público siempre deja Role="User" y los endpoints
// [Authorize(Roles = "Admin")] quedan inalcanzables. Corre una vez al arrancar
// la Api; si ya existe un Admin, no hace nada. Reutiliza IUserService.RegisterAsync
// (valida email único, hashea password) en vez de duplicar esa lógica acá.
public static class DbSeeder
{
    public static async Task SeedAdminUserAsync(
        IUserService userService, IConfiguration configuration, ILogger logger)
    {
        var users = await userService.GetAllAsync();
        if (users.Any(u => u.Role == "Admin"))
            return;

        var email = configuration["Admin:Email"]
            ?? throw new InvalidOperationException("Falta configurar 'Admin:Email' en appsettings.json.");
        var password = configuration["Admin:Password"]
            ?? throw new InvalidOperationException("Falta configurar 'Admin:Password' en appsettings.json.");

        var admin = await userService.RegisterAsync(new RegisterUserDto
        {
            Email = email,
            Name = "Admin",
            Password = password
        });
        await userService.PromoteToAdminAsync(admin.Id);

        logger.LogInformation("Usuario Admin sembrado con email {Email}.", email);
    }
}

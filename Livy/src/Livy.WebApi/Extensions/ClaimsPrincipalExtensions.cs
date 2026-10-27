// WebApi/Extensions/ClaimsPrincipalExtensions.cs
using System.Security.Claims;

namespace Livy.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    // El token emite "sub" con el Id del usuario; AddJwtBearer (MapInboundClaims = true,
    // el default) lo mapea a ClaimTypes.NameIdentifier al validar el token.
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
            throw new InvalidOperationException("El token no contiene un Id de usuario válido.");

        return userId;
    }
}

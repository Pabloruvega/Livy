// Application/Services/IJwtTokenService.cs
using Livy.Domain.Entities;

namespace Livy.Application.Services;


public interface IJwtTokenService
{
    string GenerateToken(User user);
    DateTime GetExpiration();
}
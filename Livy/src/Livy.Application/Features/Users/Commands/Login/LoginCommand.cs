// Application/Features/Users/Commands/Login/LoginCommand.cs
using Livy.Application.DTOs.User;
using MediatR;

namespace Livy.Application.Features.Users.Commands.Login;

// Devuelve null si las credenciales son inválidas (el Controller responde 401).
public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto?>;

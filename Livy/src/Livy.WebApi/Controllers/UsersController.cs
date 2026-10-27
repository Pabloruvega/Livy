// WebApi/Controllers/UsersController.cs
using Livy.Application.DTOs.User;
using Livy.Application.Features.Users.Commands.Login;
using Livy.Application.Features.Users.Commands.PromoteToAdmin;
using Livy.Application.Features.Users.Commands.RegisterUser;
using Livy.Application.Features.Users.Queries.GetAllUsers;
using Livy.Application.Features.Users.Queries.GetUserById;
using Livy.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livy.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto, CancellationToken ct)
    {
        var command = new RegisterUserCommand(dto.Email, dto.Name, dto.Password);
        var user = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var auth = await _mediator.Send(new LoginCommand(dto.Email, dto.Password), ct);
        if (auth is null)
            return Unauthorized(new { error = "Credenciales inválidas." });
        return Ok(auth);
    }

    // Listar todos los usuarios es información administrativa — solo Admin.
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllUsersQuery(), ct));

    // Cada usuario puede verse a sí mismo; un Admin, a cualquiera (si no → 403).
    // Si no existe, el handler lanza UserNotFoundException → 404 vía ErrorHandlingMiddleware.
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(
            new GetUserByIdQuery(id, User.GetUserId(), User.IsInRole("Admin")), ct));

    // Solo un Admin puede crear otros Admins — el primero sale del seed de arranque.
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/promote")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PromoteToAdmin(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new PromoteToAdminCommand(id), ct));
}
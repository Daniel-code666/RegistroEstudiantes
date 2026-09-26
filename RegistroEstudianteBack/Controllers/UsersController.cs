using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Users.Abstractions;
using RegistroEstudiante.Application.Users.Dtos;
using RegistroEstudiante.Domain.Constants;

namespace RegistroEstudianteBack.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
        => Ok(await _users.RegisterAsync(request, cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public async Task<ActionResult<PageResult<UserDetailsResponse>>> GetAll([FromQuery] UserFilter filter, CancellationToken cancellationToken)
        => Ok(await _users.GetAllAsync(filter, cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDetailsResponse>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _users.GetByIdAsync(id, cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public async Task<ActionResult<UserDetailsResponse>> Create(CreateUserRequest request, CancellationToken cancellationToken)
        => Ok(await _users.CreateAsync(request, cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDetailsResponse>> Update(int id, UpdateUserRequest request, CancellationToken cancellationToken)
        => Ok(await _users.UpdateAsync(id, request, cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        await _users.SetActiveAsync(id, false, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        await _users.SetActiveAsync(id, true, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> ResetPassword(int id, ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        await _users.ResetPasswordAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDetailsResponse>> Me(CancellationToken cancellationToken) 
        => Ok(await _users.GetCurrentUserAsync(GetCurrentUserId(), cancellationToken));

    [HttpPut("me")]
    public async Task<ActionResult<UserDetailsResponse>> UpdateProfile(UpdateProfileRequest request, CancellationToken cancellationToken) 
        => Ok(await _users.UpdateProfileAsync(GetCurrentUserId(), request, cancellationToken));

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await _users.ChangePasswordAsync(GetCurrentUserId(), request, cancellationToken);
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        if (!int.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) || id <= 0)
            throw new UnauthorizedException("No se pudo identificar al usuario.");
        return id;
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiante.Application.Roles;
using RegistroEstudiante.Application.Roles.Abstractions;
using RegistroEstudiante.Domain.Constants;

namespace RegistroEstudianteBack.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RoleNames.Admin)]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roles;

    public RoleController(IRoleService roles)
    {
        _roles = roles;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleResponse>>> Get([FromQuery] bool? active, CancellationToken cancellationToken)
        => Ok(await _roles.GetAllAsync(active, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<RoleResponse>> Create(RoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roles.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, role);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RoleResponse>> Update(int id, RoleRequest request, CancellationToken cancellationToken)
        => Ok(await _roles.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        await _roles.SetActiveAsync(id, false, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        await _roles.SetActiveAsync(id, true, cancellationToken);
        return NoContent();
    }
}

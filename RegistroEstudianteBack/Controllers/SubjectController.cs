using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Subjects.Abstractions;
using RegistroEstudiante.Application.Subjects.Dtos;
using RegistroEstudiante.Domain.Constants;

namespace RegistroEstudianteBack.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjects;

    public SubjectController(ISubjectService subjects)
    {
        _subjects = subjects;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<SubjectResponse>>> Get([FromQuery] SubjectFilter filter, CancellationToken cancellationToken)
    {
        if (!User.IsInRole(RoleNames.Admin)) filter.Active = true;
        return Ok(await _subjects.GetAllAsync(filter, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _subjects.GetByIdAsync(id, User.IsInRole(RoleNames.Admin), cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public async Task<ActionResult<SubjectResponse>> Create(SubjectRequest request, CancellationToken cancellationToken)
    {
        var subject = await _subjects.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = subject.Id }, subject);
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> Update(int id, SubjectRequest request, CancellationToken cancellationToken)
        => Ok(await _subjects.UpdateAsync(id, request, cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        await _subjects.SetActiveAsync(id, false, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        await _subjects.SetActiveAsync(id, true, cancellationToken);
        return NoContent();
    }
}

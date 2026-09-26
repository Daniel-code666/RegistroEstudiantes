using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Enrollments.Abstractions;
using RegistroEstudiante.Application.Enrollments.Dtos;
using RegistroEstudiante.Application.Subjects.Abstractions;
using RegistroEstudiante.Application.Subjects.Dtos;
using RegistroEstudiante.Domain.Constants;

namespace RegistroEstudianteBack.Controllers;

[ApiController]
[Route("api/professors/me/subjects")]
[Authorize(Roles = RoleNames.Professor)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class ProfessorsController : ControllerBase
{
    private readonly ISubjectService _subjects;
    private readonly IEnrollmentService _enrollments;

    public ProfessorsController(ISubjectService subjects, IEnrollmentService enrollments)
    {
        _subjects = subjects;
        _enrollments = enrollments;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<SubjectResponse>>> GetSubjects([FromQuery] SubjectFilter filter, CancellationToken cancellationToken)
    {
        filter.ProfessorId = GetCurrentUserId();
        filter.Assigned = true;
        return Ok(await _subjects.GetAllAsync(filter, cancellationToken));
    }

    [HttpGet("available")]
    public async Task<ActionResult<PageResult<SubjectResponse>>> GetAvailable([FromQuery] SubjectFilter filter, CancellationToken cancellationToken)
    {
        filter.ProfessorId = null;
        filter.Assigned = false;
        filter.Active = true;
        return Ok(await _subjects.GetAllAsync(filter, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<SubjectResponse>> Create(SubjectRequest request, CancellationToken cancellationToken)
        => StatusCode(StatusCodes.Status201Created, await _subjects.CreateAsync(request, cancellationToken, GetCurrentUserId()));

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> Update(int id, SubjectRequest request, CancellationToken cancellationToken)
        => Ok(await _subjects.UpdateAsync(id, request, cancellationToken, GetCurrentUserId()));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        await _subjects.SetActiveAsync(id, false, cancellationToken, GetCurrentUserId());
        return NoContent();
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        await _subjects.SetActiveAsync(id, true, cancellationToken, GetCurrentUserId());
        return NoContent();
    }

    [HttpPut("{id:int}/assignment")]
    public async Task<IActionResult> Assign(int id, CancellationToken cancellationToken)
    {
        await _subjects.SetAssignmentAsync(id, GetCurrentUserId(), true, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}/assignment")]
    public async Task<IActionResult> Unassign(int id, CancellationToken cancellationToken)
    {
        await _subjects.SetAssignmentAsync(id, GetCurrentUserId(), false, cancellationToken);
        return NoContent();
    }

    [HttpGet("{subjectId:int}/students")]
    public async Task<ActionResult<PageResult<ClassmateResponse>>> GetStudents(int subjectId, [FromQuery] Pagination pagination, CancellationToken cancellationToken)
        => Ok(await _enrollments.GetProfessorStudentsAsync(GetCurrentUserId(), subjectId, pagination, cancellationToken));

    private int GetCurrentUserId()
    {
        if (!int.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) || id <= 0)
            throw new UnauthorizedException("No se pudo identificar al usuario.");
        return id;
    }
}

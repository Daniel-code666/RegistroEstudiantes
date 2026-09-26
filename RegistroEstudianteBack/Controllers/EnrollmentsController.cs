using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Enrollments.Abstractions;
using RegistroEstudiante.Application.Enrollments.Dtos;
using RegistroEstudiante.Domain.Constants;

namespace RegistroEstudianteBack.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollments;

    public EnrollmentsController(IEnrollmentService enrollments)
    {
        _enrollments = enrollments;
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpGet("me")]
    public async Task<ActionResult<EnrollmentResponse>> GetMine(CancellationToken cancellationToken)
        => Ok(await _enrollments.GetAsync(GetCurrentUserId(), cancellationToken));

    [Authorize(Roles = RoleNames.Student)]
    [HttpPut("me")]
    public async Task<ActionResult<EnrollmentResponse>> ReplaceMine(SubjectSelectionRequest request, CancellationToken cancellationToken)
        => Ok(await _enrollments.ReplaceAsync(GetCurrentUserId(), request, cancellationToken));

    [Authorize(Roles = RoleNames.Student)]
    [HttpDelete("me/{subjectId:int}")]
    public async Task<IActionResult> RemoveMine(int subjectId, CancellationToken cancellationToken)
    {
        await _enrollments.RemoveAsync(GetCurrentUserId(), subjectId, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpGet("me/{subjectId:int}/classmates")]
    public async Task<ActionResult<PageResult<ClassmateResponse>>> GetMyClassmates(int subjectId, [FromQuery] Pagination pagination, CancellationToken cancellationToken)
        => Ok(await _enrollments.GetClassmatesAsync(GetCurrentUserId(), subjectId, pagination, cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("{userId:int}")]
    public async Task<ActionResult<EnrollmentResponse>> Get(int userId, CancellationToken cancellationToken)
        => Ok(await _enrollments.GetAsync(userId, cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("{userId:int}")]
    public async Task<ActionResult<EnrollmentResponse>> Replace(int userId, SubjectSelectionRequest request, CancellationToken cancellationToken)
        => Ok(await _enrollments.ReplaceAsync(userId, request, cancellationToken));

    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{userId:int}/{subjectId:int}")]
    public async Task<IActionResult> Remove(int userId, int subjectId, CancellationToken cancellationToken)
    {
        await _enrollments.RemoveAsync(userId, subjectId, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("{userId:int}/{subjectId:int}/classmates")]
    public async Task<ActionResult<PageResult<ClassmateResponse>>> GetClassmates(int userId, int subjectId, [FromQuery] Pagination pagination, CancellationToken cancellationToken)
        => Ok(await _enrollments.GetClassmatesAsync(userId, subjectId, pagination, cancellationToken));

    private int GetCurrentUserId()
    {
        if (!int.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) || id <= 0)
            throw new UnauthorizedException("No se pudo identificar al usuario.");
        return id;
    }
}

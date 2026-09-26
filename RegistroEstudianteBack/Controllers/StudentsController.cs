using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Enrollments.Abstractions;
using RegistroEstudiante.Application.Enrollments.Dtos;

namespace RegistroEstudianteBack.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class StudentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollments;

    public StudentsController(IEnrollmentService enrollments)
    {
        _enrollments = enrollments;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<StudentRecordResponse>>> Get([FromQuery] Pagination pagination, CancellationToken cancellationToken)
        => Ok(await _enrollments.GetStudentsAsync(pagination, cancellationToken));
}

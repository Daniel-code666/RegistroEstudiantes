using AutoMapper;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Enrollments.Abstractions;
using RegistroEstudiante.Application.Enrollments.Dtos;
using RegistroEstudiante.Application.Subjects.Dtos;
using RegistroEstudiante.Domain.Constants;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Application.Enrollments.Repositories;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollments;
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public EnrollmentService(IEnrollmentRepository enrollments, IUserRepository users, IMapper mapper)
    {
        _enrollments = enrollments;
        _users = users;
        _mapper = mapper;
    }

    public async Task<EnrollmentResponse> GetAsync(int userId, CancellationToken cancellationToken)
    {
        await ValidateStudentAsync(userId, false, cancellationToken);
        return ToResponse(userId, (await _enrollments.GetForStudentAsync(userId, cancellationToken)).Select(x => x.Subject));
    }

    public async Task<EnrollmentResponse> ReplaceAsync(int userId, SubjectSelectionRequest request, CancellationToken cancellationToken)
    {
        if (request is null) throw new ValidationException("El cuerpo de la solicitud es obligatorio.");
        request.Validate();
        await ValidateStudentAsync(userId, true, cancellationToken);
        var current = await _enrollments.GetForStudentAsync(userId, cancellationToken);
        var subjects = await ValidateSelectionAsync(request, cancellationToken);
        foreach (var enrollment in current.Where(x => !request.SubjectIds.Contains(x.SubjectId)))
            _enrollments.Remove(enrollment);
        foreach (var subject in subjects.Where(x => current.All(y => y.SubjectId != x.Id)))
            {
            var enrollment = _mapper.Map<StudentSubject>(subject);
            enrollment.UserId = userId;
            await _enrollments.AddAsync(enrollment, cancellationToken);
        }
        await _enrollments.SaveAsync(cancellationToken);
        return ToResponse(userId, subjects);
    }

    public async Task RemoveAsync(int userId, int subjectId, CancellationToken cancellationToken)
    {
        if (subjectId <= 0) throw new ValidationException("El identificador de la materia debe ser positivo.");
        await ValidateStudentAsync(userId, false, cancellationToken);
        var current = await _enrollments.GetForStudentAsync(userId, cancellationToken);
        var enrollment = current.SingleOrDefault(x => x.SubjectId == subjectId)
            ?? throw new NotFoundException("No existe la inscripcion.");
        _enrollments.Remove(enrollment);
        await _enrollments.SaveAsync(cancellationToken);
    }

    public async Task<PageResult<StudentRecordResponse>> GetStudentsAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        if (pagination is null) throw new ValidationException("La paginacion es obligatoria.");
        pagination.Validate();
        return await _enrollments.GetStudentsAsync(pagination, cancellationToken);
    }

    public async Task<PageResult<ClassmateResponse>> GetClassmatesAsync(int userId, int subjectId, Pagination pagination, CancellationToken cancellationToken)
    {
        if (pagination is null) throw new ValidationException("La paginacion es obligatoria.");
        pagination.Validate();
        if (subjectId <= 0) throw new ValidationException("El identificador de la materia debe ser positivo.");
        await ValidateStudentAsync(userId, false, cancellationToken);
        var current = await _enrollments.GetForStudentAsync(userId, cancellationToken);
        if (!current.Any(x => x.SubjectId == subjectId))
            throw new ForbiddenException("Solo se pueden consultar los companeros de las materias inscritas.");
        return await _enrollments.GetClassmatesAsync(userId, subjectId, pagination, cancellationToken);
    }

    public async Task<PageResult<ClassmateResponse>> GetProfessorStudentsAsync(int professorId, int subjectId, Pagination pagination, CancellationToken cancellationToken)
    {
        ValidationUtilities.ValidateId(professorId);
        if (subjectId <= 0) throw new ValidationException("El identificador de la materia debe ser positivo.");
        if (pagination is null) throw new ValidationException("La paginacion es obligatoria.");
        pagination.Validate();
        var professor = await _users.FindByIdAsync(professorId, cancellationToken);
        if (professor is null || !professor.Active || !professor.Role.Active || professor.Role.Name != RoleNames.Professor)
            throw new ForbiddenException("Se requiere un profesor activo.");
        var subjects = await _enrollments.GetSubjectsAsync(new[] { subjectId }, cancellationToken);
        var subject = subjects.SingleOrDefault();
        if (subject is null || !subject.Active || subject.ProfessorId != professorId)
            throw new NotFoundException("No existe la materia entre sus materias activas asignadas.");
        return await _enrollments.GetProfessorStudentsAsync(professorId, subjectId, pagination, cancellationToken);
    }

    private async Task ValidateStudentAsync(int userId, bool requireActive, CancellationToken cancellationToken)
    {
        ValidationUtilities.ValidateId(userId);
        var user = await _users.FindByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("No existe el estudiante.");
        if (user.Role.Name != RoleNames.Student || !user.Role.Active)
            throw new ValidationException("Solo los usuarios con rol Student pueden inscribirse.");
        if (requireActive && !user.Active)
            throw new ConflictException("El estudiante no se encuentra activo.");
    }

    private async Task<List<Subject>> ValidateSelectionAsync(SubjectSelectionRequest request, CancellationToken cancellationToken)
    {
        request.Validate();
        var subjects = await _enrollments.GetSubjectsAsync(request.SubjectIds, cancellationToken);
        if (subjects.Count != request.SubjectIds.Count)
            throw new NotFoundException("Una o mas materias no existen.");
        if (subjects.Any(x => !x.Active || x.Professor == null || !x.Professor.Active || !x.Professor.Role.Active || x.Professor.Role.Name != RoleNames.Professor))
            throw new ConflictException("Las materias y sus profesores deben estar activos y tener el rol correspondiente.");
        if (subjects.Select(x => x.ProfessorId).Distinct().Count() != subjects.Count)
            throw new ConflictException("El estudiante no puede seleccionar dos materias del mismo profesor.");
        if (subjects.Any(x => x.Credits != AcademicRules.CreditsPerSubject))
            throw new ConflictException("Cada materia debe tener 3 creditos.");
        return subjects;
    }

    private EnrollmentResponse ToResponse(int userId, IEnumerable<Subject> subjects)
    {
        var selected = subjects.OrderBy(x => x.Id).ToList();
        return new EnrollmentResponse
        {
            UserId = userId,
            TotalCredits = selected.Sum(x => x.Credits),
            Subjects = _mapper.Map<List<SubjectResponse>>(selected)
        };
    }
}

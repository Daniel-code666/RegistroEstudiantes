using AutoMapper;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Subjects.Abstractions;
using RegistroEstudiante.Application.Subjects.Dtos;
using RegistroEstudiante.Domain.Constants;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Application.Subjects.Repositories;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjects;
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public SubjectService(ISubjectRepository subjects, IUserRepository users, IMapper mapper)
    {
        _subjects = subjects;
        _users = users;
        _mapper = mapper;
    }

    public async Task<PageResult<SubjectResponse>> GetAllAsync(SubjectFilter filter, CancellationToken cancellationToken)
    {
        if (filter is null) throw new ValidationException("Los filtros son obligatorios.");
        filter.Validate();
        var result = await _subjects.GetAllAsync(filter, cancellationToken);
        return new PageResult<SubjectResponse>
        {
            Items = _mapper.Map<List<SubjectResponse>>(result.Items), TotalRecords = result.TotalRecords,
            PageNumber = result.PageNumber, PageSize = result.PageSize
        };
    }

    public async Task<SubjectResponse> GetByIdAsync(int id, bool includeInactive, CancellationToken cancellationToken)
    {
        var subject = await GetSubjectAsync(id, cancellationToken);
        if (!includeInactive && !subject.Active) throw new NotFoundException("No existe la materia activa.");
        return _mapper.Map<SubjectResponse>(subject);
    }

    public async Task<SubjectResponse> CreateAsync(SubjectRequest request, CancellationToken cancellationToken, int? actingProfessorId = null)
    {
        Validate(request);
        await ValidateNameAsync(request.Name, null, cancellationToken);
        var professorId = actingProfessorId ?? request.ProfessorId;
        var professor = professorId.HasValue ? await GetProfessorAsync(professorId.Value, cancellationToken) : null;
        if (professor is not null) await ValidateProfessorCapacityAsync(professor.Id, null, cancellationToken);
        var subject = _mapper.Map<Subject>(request);
        subject.ProfessorId = professor?.Id;
        subject.Professor = professor;
        await _subjects.AddAsync(subject, cancellationToken);
        return _mapper.Map<SubjectResponse>(subject);
    }

    public async Task<SubjectResponse> UpdateAsync(int id, SubjectRequest request, CancellationToken cancellationToken, int? actingProfessorId = null)
    {
        Validate(request);
        var subject = await GetSubjectAsync(id, cancellationToken);
        await ValidateOwnerAsync(subject, actingProfessorId, cancellationToken);
        var professorId = actingProfessorId ?? request.ProfessorId;
        var professor = professorId.HasValue ? await GetProfessorAsync(professorId.Value, cancellationToken) : null;
        await ValidateNameAsync(request.Name, id, cancellationToken);
        if (subject.Active && professor is not null) await ValidateProfessorCapacityAsync(professor.Id, id, cancellationToken);
        if (subject.ProfessorId != professorId)
        {
            if (professor is null) await EnsureNoEnrollmentsAsync(id, cancellationToken);
            else if (await _subjects.HasProfessorConflictAsync(id, professor.Id, cancellationToken))
                throw new ConflictException("El cambio haría que un estudiante repita profesor en sus materias.");
        }
        _mapper.Map(request, subject);
        subject.ProfessorId = professor?.Id;
        subject.Professor = professor;
        await _subjects.SaveAsync(cancellationToken);
        return _mapper.Map<SubjectResponse>(subject);
    }

    public async Task SetActiveAsync(int id, bool active, CancellationToken cancellationToken, int? actingProfessorId = null)
    {
        var subject = await GetSubjectAsync(id, cancellationToken);
        await ValidateOwnerAsync(subject, actingProfessorId, cancellationToken);
        if (subject.Active == active) return;
        if (!active) await EnsureNoEnrollmentsAsync(id, cancellationToken);
        if (active && subject.ProfessorId.HasValue)
        {
            await GetProfessorAsync(subject.ProfessorId.Value, cancellationToken);
            await ValidateProfessorCapacityAsync(subject.ProfessorId.Value, id, cancellationToken);
        }
        subject.Active = active;
        await _subjects.SaveAsync(cancellationToken);
    }

    public async Task SetAssignmentAsync(int id, int professorId, bool assigned, CancellationToken cancellationToken)
    {
        var professor = await GetProfessorAsync(professorId, cancellationToken);
        var subject = await GetSubjectAsync(id, cancellationToken);
        if (assigned)
        {
            if (!subject.Active) throw new ConflictException("Solo puede asignarse materias disponibles activas.");
            if (subject.ProfessorId.HasValue) throw new ConflictException("La materia ya tiene un profesor asignado.");
            await ValidateProfessorCapacityAsync(professorId, id, cancellationToken);
            if (await _subjects.HasProfessorConflictAsync(id, professorId, cancellationToken))
                throw new ConflictException("La asignación haría que un estudiante repita profesor.");
            subject.ProfessorId = professorId;
            subject.Professor = professor;
        }
        else
        {
            await ValidateOwnerAsync(subject, professorId, cancellationToken);
            await EnsureNoEnrollmentsAsync(id, cancellationToken);
            subject.ProfessorId = null;
            subject.Professor = null;
        }
        await _subjects.SaveAsync(cancellationToken);
    }

    private async Task ValidateOwnerAsync(Subject subject, int? professorId, CancellationToken cancellationToken)
    {
        if (!professorId.HasValue) return;
        await GetProfessorAsync(professorId.Value, cancellationToken);
        if (subject.ProfessorId != professorId)
            throw new ForbiddenException("Solo puede gestionar sus propias materias.");
    }

    private async Task EnsureNoEnrollmentsAsync(int id, CancellationToken cancellationToken)
    {
        if (await _subjects.HasEnrollmentsAsync(id, cancellationToken))
            throw new ConflictException("No se puede desactivar o desasignar una materia con estudiantes inscritos.");
    }

    private async Task<Subject> GetSubjectAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0) throw new ValidationException("El identificador de la materia debe ser positivo.");
        return await _subjects.FindByIdAsync(id, cancellationToken) ?? throw new NotFoundException("No existe la materia.");
    }

    private async Task<User> GetProfessorAsync(int id, CancellationToken cancellationToken)
    {
        var professor = await _users.FindTrackedByIdAsync(id, cancellationToken);
        if (professor is null || !professor.Active || !professor.Role.Active || professor.Role.Name != RoleNames.Professor)
            throw new ValidationException("La materia debe asignarse a un usuario activo con rol Professor.");
        return professor;
    }

    private async Task ValidateProfessorCapacityAsync(int professorId, int? excludeId, CancellationToken cancellationToken)
    {
        if (await _subjects.CountForProfessorAsync(professorId, excludeId, cancellationToken) >= AcademicRules.MaxSubjectsPerProfessor)
            throw new ConflictException("Un profesor puede dictar un máximo de 2 materias activas.");
    }

    private async Task ValidateNameAsync(string name, int? id, CancellationToken cancellationToken)
    {
        if (await _subjects.NameExistsAsync(name.Trim(), id, cancellationToken))
            throw new ConflictException("Ya existe una materia con ese nombre.");
    }

    private static void Validate(SubjectRequest? request)
    {
        if (request is null) throw new ValidationException("El cuerpo de la solicitud es obligatorio.");
        request.Validate();
    }
}

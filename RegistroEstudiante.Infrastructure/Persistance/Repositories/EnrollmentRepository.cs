using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Enrollments.Dtos;
using RegistroEstudiante.Domain.Constants;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Infrastructure.Persistance.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public EnrollmentRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public Task<List<StudentSubject>> GetForStudentAsync(int userId, CancellationToken cancellationToken)
        => _db.StudentSubjects.Include(x => x.Subject).ThenInclude(x => x.Professor).ThenInclude(x => x!.Role)
            .Where(x => x.UserId == userId).OrderBy(x => x.SubjectId).ToListAsync(cancellationToken);

    public Task<List<Subject>> GetSubjectsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken)
        => _db.Subjects.Include(x => x.Professor).ThenInclude(x => x!.Role)
            .Where(x => ids.Contains(x.Id)).OrderBy(x => x.Id).ToListAsync(cancellationToken);

    public async Task<PageResult<StudentRecordResponse>> GetStudentsAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var query = _db.Users.AsNoTracking().Where(x => x.Active && x.Role.Active && x.Role.Name == RoleNames.Student);
        var count = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Id).Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ProjectTo<StudentRecordResponse>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        return new PageResult<StudentRecordResponse>
        {
            Items = items, TotalRecords = count, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<PageResult<ClassmateResponse>> GetClassmatesAsync(int userId, int subjectId, Pagination pagination, CancellationToken cancellationToken)
    {
        var query = _db.Users.AsNoTracking().Where(x => x.Id != userId && x.Active && x.Role.Active &&
            x.Role.Name == RoleNames.Student && x.StudentSubjects.Any(y => y.SubjectId == subjectId));
        var count = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.LastName).ThenBy(x => x.Id)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize)
            .ProjectTo<ClassmateResponse>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        return new PageResult<ClassmateResponse>
        {
            Items = items, TotalRecords = count, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<PageResult<ClassmateResponse>> GetProfessorStudentsAsync(int professorId, int subjectId, Pagination pagination, CancellationToken cancellationToken)
    {
        var query = _db.Users.AsNoTracking().Where(x => x.Active && x.Role.Active &&
            x.Role.Name == RoleNames.Student && x.StudentSubjects.Any(y =>
                y.SubjectId == subjectId && y.Subject.ProfessorId == professorId && y.Subject.Active));
        var count = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.LastName).ThenBy(x => x.Id)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize)
            .ProjectTo<ClassmateResponse>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        return new PageResult<ClassmateResponse>
        {
            Items = items, TotalRecords = count, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task AddAsync(StudentSubject enrollment, CancellationToken cancellationToken)
        => await _db.StudentSubjects.AddAsync(enrollment, cancellationToken);

    public void Remove(StudentSubject enrollment)
        => _db.StudentSubjects.Remove(enrollment);

    public async Task SaveAsync(CancellationToken cancellationToken)
        => await _db.SaveChangesAsync(cancellationToken);
}

using Microsoft.EntityFrameworkCore;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Subjects.Dtos;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Infrastructure.Persistance.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly ApplicationDbContext _db;

    public SubjectRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PageResult<Subject>> GetAllAsync(SubjectFilter filter, CancellationToken cancellationToken)
    {
        var query = _db.Subjects.AsNoTracking().Include(x => x.Professor).AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(x => x.Name.Contains(search));
        }
        if (filter.Assigned.HasValue) query = query.Where(x => x.ProfessorId.HasValue == filter.Assigned.Value);
        if (filter.ProfessorId.HasValue) query = query.Where(x => x.ProfessorId == filter.ProfessorId.Value);
        if (filter.Active.HasValue) query = query.Where(x => x.Active == filter.Active.Value);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Id).Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize).ToListAsync(cancellationToken);
        return new PageResult<Subject>
        {
            Items = items, TotalRecords = total, PageNumber = filter.PageNumber, PageSize = filter.PageSize
        };
    }

    public Task<Subject?> FindByIdAsync(int id, CancellationToken cancellationToken)
        => _db.Subjects.Include(x => x.Professor).ThenInclude(x => x!.Role).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken)
        => _db.Subjects.AnyAsync(x => x.Name == name && (!excludeId.HasValue || x.Id != excludeId), cancellationToken);

    public Task<int> CountForProfessorAsync(int professorId, int? excludeId, CancellationToken cancellationToken)
        => _db.Subjects.CountAsync(x => x.Active && x.ProfessorId == professorId && (!excludeId.HasValue || x.Id != excludeId), cancellationToken);

    public Task<bool> HasEnrollmentsAsync(int subjectId, CancellationToken cancellationToken)
        => _db.StudentSubjects.AnyAsync(x => x.SubjectId == subjectId, cancellationToken);

    public Task<bool> HasProfessorConflictAsync(int subjectId, int professorId, CancellationToken cancellationToken)
        => _db.StudentSubjects.AnyAsync(x => x.SubjectId == subjectId &&
            _db.StudentSubjects.Any(y => y.UserId == x.UserId && y.SubjectId != subjectId && y.Subject.ProfessorId == professorId), cancellationToken);

    public async Task AddAsync(Subject subject, CancellationToken cancellationToken)
    {
        _db.Subjects.Add(subject);
        await SaveAsync(cancellationToken);
    }

    public Task SaveAsync(CancellationToken cancellationToken)
        => PersistenceUtilities.SaveAsync(_db, cancellationToken);
}

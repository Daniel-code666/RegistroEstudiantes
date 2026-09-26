using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Users.Dtos;
using RegistroEstudiante.Domain.Constants;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Infrastructure.Persistance.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        => _db.Users.AsNoTracking().Include(x => x.Role)
            .SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

    public Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken)
        => _db.Users.AsNoTracking().Include(x => x.Role).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<User?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken)
        => _db.Users.Include(x => x.Role).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PageResult<User>> GetAllAsync(UserFilter filter, CancellationToken cancellationToken)
    {
        var query = _db.Users.AsNoTracking().Include(x => x.Role).AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(x => x.Name.Contains(search) || x.LastName.Contains(search) || x.Email.Contains(search));
        }
        if (!string.IsNullOrWhiteSpace(filter.Role))
            query = query.Where(x => x.Role.Name == filter.Role);
        if (filter.Active.HasValue)
            query = query.Where(x => x.Active == filter.Active.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Id).Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize).ToListAsync(cancellationToken);
        return new PageResult<User>
        {
            Items = items, TotalRecords = total, PageNumber = filter.PageNumber, PageSize = filter.PageSize
        };
    }

    public Task<bool> HasOtherActiveAdminAsync(int id, CancellationToken cancellationToken)
        => _db.Users.AnyAsync(x => x.Id != id && x.Active && x.Role.Active && x.Role.Name == RoleNames.Admin, cancellationToken);

    public Task<bool> HasAdminAsync(CancellationToken cancellationToken)
        => _db.Users.AnyAsync(x => x.Role.Name == RoleNames.Admin, cancellationToken);

    public Task<bool> HasTaughtSubjectsAsync(int id, CancellationToken cancellationToken)
        => _db.Subjects.AnyAsync(x => x.ProfessorId == id, cancellationToken);

    public Task<bool> HasEnrollmentsAsync(int id, CancellationToken cancellationToken)
        => _db.StudentSubjects.AnyAsync(x => x.UserId == id, cancellationToken);

    public Task<bool> HasActiveTaughtSubjectsAsync(int id, CancellationToken cancellationToken)
        => _db.Subjects.AnyAsync(x => x.ProfessorId == id && x.Active, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Add(user);
        await SaveAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new ConflictException("El correo o el documento ya existen.");
        }
    }
}

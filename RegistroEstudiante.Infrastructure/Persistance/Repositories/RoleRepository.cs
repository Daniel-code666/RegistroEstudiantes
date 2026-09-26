using Microsoft.EntityFrameworkCore;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Infrastructure.Persistance.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _db;

    public RoleRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Role?> FindByNameAsync(string name, CancellationToken cancellationToken)
        => _db.Roles.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

    public Task<Role?> FindByIdAsync(int id, CancellationToken cancellationToken)
        => _db.Roles.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Role>> GetAllAsync(bool? active, CancellationToken cancellationToken)
    {
        var query = _db.Roles.AsNoTracking();
        if (active.HasValue) query = query.Where(x => x.Active == active.Value);
        return await query.OrderBy(x => x.Id).ToListAsync(cancellationToken);
    }

    public Task<bool> HasUsersAsync(int id, CancellationToken cancellationToken)
        => _db.Users.AnyAsync(x => x.RoleId == id, cancellationToken);

    public async Task AddAsync(Role role, CancellationToken cancellationToken)
    {
        _db.Roles.Add(role);
        await SaveAsync(cancellationToken);
    }

    public Task SaveAsync(CancellationToken cancellationToken)
        => PersistenceUtilities.SaveAsync(_db, cancellationToken);
}

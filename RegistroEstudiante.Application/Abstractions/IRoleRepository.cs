using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Application.Abstractions
{
    public interface IRoleRepository
    {
        Task<Role?> FindByNameAsync(string name, CancellationToken cancellationToken);
        Task<IReadOnlyList<Role>> GetAllAsync(bool? active, CancellationToken cancellationToken);
        Task<Role?> FindByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> HasUsersAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(Role role, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
    }
}

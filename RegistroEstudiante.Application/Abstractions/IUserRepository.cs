using RegistroEstudiante.Domain.Entities;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Users.Dtos;

namespace RegistroEstudiante.Application.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
        Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> FindTrackedByIdAsync(int id, CancellationToken cancellationToken);
        Task<PageResult<User>> GetAllAsync(UserFilter filter, CancellationToken cancellationToken);
        Task<bool> HasOtherActiveAdminAsync(int id, CancellationToken cancellationToken);
        Task<bool> HasAdminAsync(CancellationToken cancellationToken);
        Task<bool> HasTaughtSubjectsAsync(int id, CancellationToken cancellationToken);
        Task<bool> HasActiveTaughtSubjectsAsync(int id, CancellationToken cancellationToken);
        Task<bool> HasEnrollmentsAsync(int id, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
    }
}

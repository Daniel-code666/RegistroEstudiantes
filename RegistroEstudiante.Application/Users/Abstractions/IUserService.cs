using RegistroEstudiante.Application.Users.Dtos;
using RegistroEstudiante.Application.Common;

namespace RegistroEstudiante.Application.Users.Abstractions;

public interface IUserService
{
    Task ResetPasswordAsync(int id, ResetPasswordRequest request, CancellationToken cancellationToken);
    Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<UserDetailsResponse> GetCurrentUserAsync(int userId, CancellationToken cancellationToken);
    Task<PageResult<UserDetailsResponse>> GetAllAsync(UserFilter filter, CancellationToken cancellationToken);
    Task<UserDetailsResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<UserDetailsResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<UserDetailsResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task SetActiveAsync(int id, bool active, CancellationToken cancellationToken);
    Task<UserDetailsResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken);
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken);
}

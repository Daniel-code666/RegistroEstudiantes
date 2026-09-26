using RegistroEstudiante.Application.Authentication.Dtos;

namespace RegistroEstudiante.Application.Authentication.Abstractions;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}

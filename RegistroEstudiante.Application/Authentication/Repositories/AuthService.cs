using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Authentication.Abstractions;
using RegistroEstudiante.Application.Authentication.Dtos;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Common.Security;
using RegistroEstudiante.Application.Users.Dtos;

namespace RegistroEstudiante.Application.Authentication.Repositories;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly JwtTokenGenerator _tokens;

    public AuthService(IUserRepository users, JwtTokenGenerator tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _users.FindByEmailAsync(request.Email.Trim().ToUpperInvariant(), cancellationToken);
        if (user is null || !user.Active || !user.Role.Active || !PasswordUtility.Verify(user, request.Password))
            throw new UnauthorizedException("Correo o contraseña incorrectos.");

        var (accessToken, expiresAtUtc) = _tokens.CreateToken(user);

        return new LoginResponse
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresAtUtc = expiresAtUtc,
            User = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.Name
            }
        };
    }

}

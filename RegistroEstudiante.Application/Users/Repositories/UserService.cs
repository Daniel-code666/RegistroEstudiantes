using AutoMapper;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Common.Security;
using RegistroEstudiante.Application.Users.Abstractions;
using RegistroEstudiante.Application.Users.Dtos;
using RegistroEstudiante.Domain.Constants;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Application.Users.Repositories;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IMapper _mapper;

    public UserService(IUserRepository users, IRoleRepository roles, IMapper mapper)
    {
        _users = users;
        _roles = roles;
        _mapper = mapper;
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        return _mapper.Map<UserResponse>(await CreateUserAsync(request, RoleNames.Student, cancellationToken));
    }

    public async Task<UserDetailsResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        return _mapper.Map<UserDetailsResponse>(await CreateUserAsync(request, request.Role, cancellationToken));
    }

    private async Task<User> CreateUserAsync(RegisterRequest request, string roleName, CancellationToken cancellationToken)
    {
        await ValidateEmailAsync(request.Email, null, cancellationToken);
        var role = await GetRoleAsync(roleName, cancellationToken);
        var user = _mapper.Map<User>(request);
        user.RoleId = role.Id;
        user.Role = role;
        user.PasswordHash = PasswordUtility.Hash(user, request.Password);
        await _users.AddAsync(user, cancellationToken);
        return user;
    }

    public async Task<PageResult<UserDetailsResponse>> GetAllAsync(UserFilter filter, CancellationToken cancellationToken)
    {
        if (filter is null) throw new ValidationException("Los filtros son obligatorios.");
        filter.Validate();
        var page = await _users.GetAllAsync(filter, cancellationToken);
        return new PageResult<UserDetailsResponse>
        {
            Items = _mapper.Map<List<UserDetailsResponse>>(page.Items),
            TotalRecords = page.TotalRecords, PageNumber = page.PageNumber, PageSize = page.PageSize
        };
    }

    public async Task<UserDetailsResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        ValidationUtilities.ValidateId(id);
        var user = await _users.FindByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("No existe el usuario.");
        return _mapper.Map<UserDetailsResponse>(user);
    }

    public async Task<UserDetailsResponse> GetCurrentUserAsync(int userId, CancellationToken cancellationToken)
    {
        ValidationUtilities.ValidateId(userId);
        var user = await _users.FindByIdAsync(userId, cancellationToken);
        EnsureAvailable(user);
        return _mapper.Map<UserDetailsResponse>(user);
    }

    public async Task<UserDetailsResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        ValidationUtilities.ValidateId(id);
        Validate(request);
        var user = await GetTrackedAsync(id, cancellationToken);
        var role = await GetRoleAsync(request.Role, cancellationToken);
        if (user.RoleId != role.Id)
        {
            await ProtectLastAdminAsync(user, cancellationToken);
            if (await _users.HasTaughtSubjectsAsync(id, cancellationToken))
                throw new ConflictException("No se puede cambiar el rol de un profesor con materias asignadas.");
            if (await _users.HasEnrollmentsAsync(id, cancellationToken))
                throw new ConflictException("No se puede cambiar el rol de un estudiante con inscripciones.");
            user.Role = role;
            user.RoleId = role.Id;
            user.TokenVersion++;
        }
        await ValidateEmailAsync(request.Email, id, cancellationToken);
        _mapper.Map(request, user);
        await _users.SaveAsync(cancellationToken);
        return _mapper.Map<UserDetailsResponse>(user);
    }

    public async Task SetActiveAsync(int id, bool active, CancellationToken cancellationToken)
    {
        ValidationUtilities.ValidateId(id);
        var user = await GetTrackedAsync(id, cancellationToken);
        if (user.Active == active) return;
        if (!active) await ProtectLastAdminAsync(user, cancellationToken);
        if (!active && await _users.HasActiveTaughtSubjectsAsync(id, cancellationToken))
            throw new ConflictException("No se puede desactivar un profesor con materias activas asignadas.");
        if (active && !user.Role.Active)
            throw new ConflictException("El rol del usuario no se encuentra activo.");
        user.Active = active;
        user.TokenVersion++;
        await _users.SaveAsync(cancellationToken);
    }

    public async Task<UserDetailsResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        ValidationUtilities.ValidateId(userId);
        Validate(request);
        var user = await GetTrackedAsync(userId, cancellationToken);
        EnsureAvailable(user);
        await ValidateEmailAsync(request.Email, userId, cancellationToken);
        _mapper.Map(request, user);
        await _users.SaveAsync(cancellationToken);
        return _mapper.Map<UserDetailsResponse>(user);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        ValidationUtilities.ValidateId(userId);
        if (request is null) throw new ValidationException("El cuerpo de la solicitud es obligatorio.");
        request.Validate();
        var user = await GetTrackedAsync(userId, cancellationToken);
        EnsureAvailable(user);
        if (!PasswordUtility.Verify(user, request.CurrentPassword))
            throw new ValidationException("La clave actual no es correcta.");
        user.PasswordHash = PasswordUtility.Hash(user, request.NewPassword);
        user.TokenVersion++;
        await _users.SaveAsync(cancellationToken);
    }

    public async Task ResetPasswordAsync(int id, ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        ValidationUtilities.ValidateId(id);
        if (request is null) throw new ValidationException("El cuerpo de la solicitud es obligatorio.");
        request.Validate();
        var user = await GetTrackedAsync(id, cancellationToken);
        user.PasswordHash = PasswordUtility.Hash(user, request.NewPassword);
        user.TokenVersion++;
        await _users.SaveAsync(cancellationToken);
    }

    private async Task ProtectLastAdminAsync(User user, CancellationToken cancellationToken)
    {
        if (user.Active && user.Role.Name == RoleNames.Admin
            && !await _users.HasOtherActiveAdminAsync(user.Id, cancellationToken))
            throw new ConflictException("Debe permanecer al menos un administrador activo.");
    }

    private async Task ValidateEmailAsync(string email, int? userId, CancellationToken cancellationToken)
    {
        var existing = await _users.FindByEmailAsync(email.Trim().ToUpperInvariant(), cancellationToken);
        if (existing is not null && existing.Id != userId)
            throw new ConflictException("El correo ya existe.");
    }

    private async Task<Role> GetRoleAsync(string name, CancellationToken cancellationToken)
    {
        var role = await _roles.FindByNameAsync(name.Trim(), cancellationToken);
        if (role is null || !role.Active)
            throw new ValidationException("El rol no existe o no se encuentra activo.");
        return role;
    }

    private async Task<User> GetTrackedAsync(int id, CancellationToken cancellationToken)
        => await _users.FindTrackedByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("No existe el usuario.");

    private static void EnsureAvailable(User? user)
    {
        if (user is null || !user.Active || !user.Role.Active)
            throw new UnauthorizedException("El usuario no se encuentra disponible.");
    }

    private static void Validate(UserFieldsRequest? request)
    {
        if (request is null) throw new ValidationException("El cuerpo de la solicitud es obligatorio.");
        request.Validate();
    }
}

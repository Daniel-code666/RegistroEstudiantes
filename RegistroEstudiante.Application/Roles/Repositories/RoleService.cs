using AutoMapper;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Roles.Abstractions;
using RegistroEstudiante.Domain.Constants;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Application.Roles.Repositories;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roles;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository roles, IMapper mapper)
    {
        _roles = roles;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<RoleResponse>> GetAllAsync(bool? active, CancellationToken cancellationToken)
        => _mapper.Map<List<RoleResponse>>(await _roles.GetAllAsync(active, cancellationToken));

    public async Task<RoleResponse> CreateAsync(RoleRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        await ValidateNameAsync(request.Name, null, cancellationToken);
        var role = _mapper.Map<Role>(request);
        await _roles.AddAsync(role, cancellationToken);
        return _mapper.Map<RoleResponse>(role);
    }

    public async Task<RoleResponse> UpdateAsync(int id, RoleRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        var role = await GetRoleAsync(id, cancellationToken);
        if (role.Name != request.Name.Trim())
        {
            if (IsSystemRole(role.Name) || await _roles.HasUsersAsync(id, cancellationToken))
                throw new ConflictException("No se puede renombrar un rol del sistema o asignado a usuarios.");
        }
        await ValidateNameAsync(request.Name, id, cancellationToken);
        _mapper.Map(request, role);
        await _roles.SaveAsync(cancellationToken);
        return _mapper.Map<RoleResponse>(role);
    }

    public async Task SetActiveAsync(int id, bool active, CancellationToken cancellationToken)
    {
        var role = await GetRoleAsync(id, cancellationToken);
        if (role.Active == active) return;
        if (!active && (IsSystemRole(role.Name) || await _roles.HasUsersAsync(id, cancellationToken)))
            throw new ConflictException("No se puede desactivar un rol del sistema o asignado a usuarios.");
        role.Active = active;
        await _roles.SaveAsync(cancellationToken);
    }

    private async Task<Role> GetRoleAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0) throw new ValidationException("El identificador del rol debe ser positivo.");
        return await _roles.FindByIdAsync(id, cancellationToken) ?? throw new NotFoundException("No existe el rol.");
    }

    private async Task ValidateNameAsync(string name, int? id, CancellationToken cancellationToken)
    {
        var existing = await _roles.FindByNameAsync(name.Trim(), cancellationToken);
        if (existing is not null && existing.Id != id)
            throw new ConflictException("Ya existe un rol con ese nombre.");
    }

    private static bool IsSystemRole(string name)
        => new[] { RoleNames.Admin, RoleNames.Professor, RoleNames.Student }.Contains(name, StringComparer.OrdinalIgnoreCase);

    private static void Validate(RoleRequest? request)
    {
        if (request is null) throw new ValidationException("El cuerpo de la solicitud es obligatorio.");
        request.Validate();
    }
}

namespace RegistroEstudiante.Application.Roles.Abstractions
{
    public interface IRoleService
    {
        Task<IReadOnlyList<RoleResponse>> GetAllAsync(bool? active, CancellationToken cancellationToken);
        Task<RoleResponse> CreateAsync(RoleRequest request, CancellationToken cancellationToken);
        Task<RoleResponse> UpdateAsync(int id, RoleRequest request, CancellationToken cancellationToken);
        Task SetActiveAsync(int id, bool active, CancellationToken cancellationToken);
    }
}

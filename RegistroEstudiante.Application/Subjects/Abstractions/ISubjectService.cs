using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Subjects.Dtos;

namespace RegistroEstudiante.Application.Subjects.Abstractions
{
    public interface ISubjectService
    {
        Task<PageResult<SubjectResponse>> GetAllAsync(SubjectFilter filter, CancellationToken cancellationToken);
        Task<SubjectResponse> GetByIdAsync(int id, bool includeInactive, CancellationToken cancellationToken);
        Task<SubjectResponse> CreateAsync(SubjectRequest request, CancellationToken cancellationToken, int? actingProfessorId = null);
        Task<SubjectResponse> UpdateAsync(int id, SubjectRequest request, CancellationToken cancellationToken, int? actingProfessorId = null);
        Task SetActiveAsync(int id, bool active, CancellationToken cancellationToken, int? actingProfessorId = null);
        Task SetAssignmentAsync(int id, int professorId, bool assigned, CancellationToken cancellationToken);
    }
}

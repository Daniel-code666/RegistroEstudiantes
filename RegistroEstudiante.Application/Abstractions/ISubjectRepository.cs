using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Subjects.Dtos;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Application.Abstractions
{
    public interface ISubjectRepository
    {
        Task<PageResult<Subject>> GetAllAsync(SubjectFilter filter, CancellationToken cancellationToken);
        Task<Subject?> FindByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken);
        Task<int> CountForProfessorAsync(int professorId, int? excludeId, CancellationToken cancellationToken);
        Task<bool> HasEnrollmentsAsync(int subjectId, CancellationToken cancellationToken);
        Task<bool> HasProfessorConflictAsync(int subjectId, int professorId, CancellationToken cancellationToken);
        Task AddAsync(Subject subject, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
    }
}

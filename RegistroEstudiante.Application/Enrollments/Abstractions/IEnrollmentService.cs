using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Enrollments.Dtos;

namespace RegistroEstudiante.Application.Enrollments.Abstractions;

public interface IEnrollmentService
{
    Task<PageResult<ClassmateResponse>> GetProfessorStudentsAsync(int professorId, int subjectId, Pagination pagination, CancellationToken cancellationToken);
    Task<EnrollmentResponse> GetAsync(int userId, CancellationToken cancellationToken);
    Task<EnrollmentResponse> ReplaceAsync(int userId, SubjectSelectionRequest request, CancellationToken cancellationToken);
    Task RemoveAsync(int userId, int subjectId, CancellationToken cancellationToken);
    Task<PageResult<StudentRecordResponse>> GetStudentsAsync(Pagination pagination, CancellationToken cancellationToken);
    Task<PageResult<ClassmateResponse>> GetClassmatesAsync(int userId, int subjectId, Pagination pagination, CancellationToken cancellationToken);
}

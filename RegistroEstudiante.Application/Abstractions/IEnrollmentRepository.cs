using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Application.Enrollments.Dtos;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Application.Abstractions;

public interface IEnrollmentRepository
{
    Task<PageResult<ClassmateResponse>> GetProfessorStudentsAsync(int professorId, int subjectId, Pagination pagination, CancellationToken cancellationToken);
    Task<List<StudentSubject>> GetForStudentAsync(int userId, CancellationToken cancellationToken);
    Task<List<Subject>> GetSubjectsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken);
    Task<PageResult<StudentRecordResponse>> GetStudentsAsync(Pagination pagination, CancellationToken cancellationToken);
    Task<PageResult<ClassmateResponse>> GetClassmatesAsync(int userId, int subjectId, Pagination pagination, CancellationToken cancellationToken);
    Task AddAsync(StudentSubject enrollment, CancellationToken cancellationToken);
    void Remove(StudentSubject enrollment);
    Task SaveAsync(CancellationToken cancellationToken);
}

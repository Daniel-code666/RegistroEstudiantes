using RegistroEstudiante.Application.Subjects.Dtos;

namespace RegistroEstudiante.Application.Enrollments.Dtos;

public class EnrollmentResponse
{
    public int UserId { get; set; }
    public int TotalCredits { get; set; }
    public IReadOnlyList<SubjectResponse> Subjects { get; set; } = [];
}

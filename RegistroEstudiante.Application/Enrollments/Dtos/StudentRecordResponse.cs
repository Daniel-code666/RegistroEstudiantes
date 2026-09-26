namespace RegistroEstudiante.Application.Enrollments.Dtos;

public class StudentRecordResponse
{
    public string Name { get; set; } = string.Empty;
    public List<StudentSubjectResponse> Subjects { get; set; } = [];
}

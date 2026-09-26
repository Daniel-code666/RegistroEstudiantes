namespace RegistroEstudiante.Application.Subjects.Dtos;

public class SubjectResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int? ProfessorId { get; set; }
    public string ProfessorName { get; set; } = string.Empty;
    public bool Active { get; set; }
}

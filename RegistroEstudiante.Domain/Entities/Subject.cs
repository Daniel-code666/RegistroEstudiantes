namespace RegistroEstudiante.Domain.Entities
{
    public class Subject : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; } = 3;
        public int? ProfessorId { get; set; }
        public User? Professor { get; set; }
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
    }
}

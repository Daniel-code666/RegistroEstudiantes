using RegistroEstudiante.Domain.Enums;

namespace RegistroEstudiante.Domain.Entities
{
    public class User : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NormalizedEmail { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int TokenVersion { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public TipoIdentificacion IdentificationType { get; set; }
        public string IdentificationNumber { get; set; } = string.Empty;
        public ICollection<StudentSubject> StudentSubjects { get; set; } = [];
        public ICollection<Subject> TaughtSubjects { get; set; } = [];
        public IReadOnlyCollection<Subject> Subjects
            => [.. StudentSubjects.Select(x => x.Subject)];
    }
}

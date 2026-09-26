namespace RegistroEstudiante.Domain.Entities
{
    public abstract class AuditTable
    {
        public DateTime CreationDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }
    }
}

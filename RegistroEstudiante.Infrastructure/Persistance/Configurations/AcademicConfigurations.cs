using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Infrastructure.Persistance.Configurations;

internal class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ConfigureAudit();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Credits).HasDefaultValue(3);
        builder.ToTable("Subjects", table => table.HasCheckConstraint("CK_Subjects_Credits", "[Credits] = 3"));
        builder.HasOne(x => x.Professor).WithMany(x => x.TaughtSubjects).HasForeignKey(x => x.ProfessorId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal class StudentSubjectConfiguration : IEntityTypeConfiguration<StudentSubject>
{
    public void Configure(EntityTypeBuilder<StudentSubject> builder)
    {
        builder.ConfigureAudit();
        builder.ToTable("StudentSubjects");
        builder.HasKey(x => new { x.SubjectId, x.UserId });
        builder.HasIndex(x => new { x.UserId, x.SubjectId }).IsUnique();
        builder.HasOne(x => x.User).WithMany(x => x.StudentSubjects).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Subject).WithMany(x => x.StudentSubjects).HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
    }
}

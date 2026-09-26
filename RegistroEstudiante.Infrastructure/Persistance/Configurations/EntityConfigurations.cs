using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegistroEstudiante.Domain.Entities;
using RegistroEstudiante.Domain.Constants;

namespace RegistroEstudiante.Infrastructure.Persistance.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ConfigureAudit();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(254).IsRequired();
            builder.Property(x => x.NormalizedEmail).HasMaxLength(254).IsRequired();
            builder.HasIndex(x => x.NormalizedEmail).IsUnique();
            builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            builder.Property(x => x.IdentificationNumber).HasMaxLength(30).IsRequired();
            builder.HasIndex(x => new { x.IdentificationType, x.IdentificationNumber }).IsUnique();
            builder.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
            builder.Ignore(x => x.Subjects);
        }
    }

    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ConfigureAudit();
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();
            var created = new DateTime(2026, 9, 25, 0, 0, 0, DateTimeKind.Utc);
            builder.HasData(
                new { Id = 1, Name = RoleNames.Student, Description = "Estudiante", Active = true, CreationDate = created },
                new { Id = 2, Name = RoleNames.Admin, Description = "Administrador", Active = true, CreationDate = created },
                new { Id = 3, Name = RoleNames.Professor, Description = "Profesor", Active = true, CreationDate = created });
        }
    }

    internal static class AuditConfiguration
    {
        public static void ConfigureAudit<T>(this EntityTypeBuilder<T> builder) where T : AuditTable
        {
            builder.Property(x => x.CreationDate).HasColumnType("datetime").HasDefaultValueSql("GETUTCDATE()")
                .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasColumnType("datetime")
                .HasConversion(x => x, x => x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : x);
        }
    }


}

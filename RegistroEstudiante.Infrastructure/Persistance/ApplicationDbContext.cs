using Microsoft.EntityFrameworkCore;
using RegistroEstudiante.Domain.Entities;

namespace RegistroEstudiante.Infrastructure.Persistance
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users
            => Set<User>();
        public DbSet<Role> Roles
            => Set<Role>();
        public DbSet<Subject> Subjects
            => Set<Subject>();
        public DbSet<StudentSubject> StudentSubjects
            => Set<StudentSubject>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        private void ApplyAudit()
        {
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<AuditTable>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(x => x.CreationDate).CurrentValue = now;
                    entry.Property(x => x.UpdatedDate).CurrentValue = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(x => x.CreationDate).IsModified = false;
                    entry.Property(x => x.UpdatedDate).CurrentValue = now;
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyAudit();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}

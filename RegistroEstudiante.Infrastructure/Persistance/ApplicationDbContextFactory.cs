using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RegistroEstudiante.Infrastructure.Persistance;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=RegistroEstudiantes;Integrated Security=true;TrustServerCertificate=true";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>();
        InfrastructureDependencyInjection.ConfigureDatabase(options, connectionString);
        return new ApplicationDbContext(options.Options);
    }
}

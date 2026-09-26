using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegistroEstudiante.Application.Abstractions;
using RegistroEstudiante.Infrastructure.Persistance;
using RegistroEstudiante.Infrastructure.Persistance.Repositories;

namespace RegistroEstudiante.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options => ConfigureDatabase(options, connectionString));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

            return services;
        }

        public static void ConfigureDatabase(DbContextOptionsBuilder optionsBuilder, string ConnectionString)
            => optionsBuilder.UseSqlServer(ConnectionString);
    }
}

using Microsoft.Extensions.Diagnostics.HealthChecks;
using RegistroEstudiante.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace RegistroEstudianteBack.Health
{
    public class DatabaseHealthCheck(ApplicationDbContext db) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if ((await db.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                    return HealthCheckResult.Unhealthy("Hay migraciones pendientes.");
                // Comprobar que la tabla es consultable; no requiere que existan clientes.
                _ = await db.Users.AnyAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex) { return HealthCheckResult.Unhealthy("Base de datos no disponible.", ex); }
        }
    }
}

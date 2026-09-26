using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RegistroEstudiante.Application.Common;

namespace RegistroEstudiante.Infrastructure.Persistance;

internal static class PersistenceUtilities
{
    public static async Task SaveAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.GetBaseException() is SqlException { Number: 2601 or 2627 })
        {
            throw new ConflictException("El registro ya existe o contiene valores duplicados.");
        }
    }
}
